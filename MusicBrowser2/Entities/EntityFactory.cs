using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities.Kinds;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;

namespace MusicBrowser.Entities
{
    public class EntityFactory
    {
        private static readonly long FirstCompatibleCache = Util.Helper.ParseVersion("2.2.2.4");
        private readonly ICacheEngine _cacheEngine = CacheEngineFactory.GetCacheEngine();

        #region IEntityFactory Members

        public IEntity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        bool IsNotStale(DateTime cacheDate, params DateTime[] comparisons)
        {
            return comparisons.All(d => d <= cacheDate);
        }

        // works out where the metadata file is (if there is one)
        string MetadataPath(FileSystemItem item)
        {
            string metadataPath = Directory.GetParent(item.FullPath).FullName;

            string metadataLocal = metadataPath + "\\" + item.Name + "\\metadata.xml";
            if (File.Exists(metadataLocal))
            {
                return metadataLocal;
            }
            string metadataInParent =  metadataPath + "\\metadata\\" + item.Name + ".xml";
            if (File.Exists(metadataInParent))
            {
                return metadataInParent;
            }
            return string.Empty;
        }

        public IEntity GetItem(FileSystemItem item)
        {
            // don't waste time trying to determine a known not entity
            if (Util.Helper.IsNotEntity(item.FullPath)) { return new Unknown(); }
            if (item.Name.ToLower() == "metadata") { return new Unknown(); }

            IEntity entity;
            string metadataFile = MetadataPath(item);

            string key = Util.Helper.GetCacheKey(item.FullPath);
            FileSystemItem metadata = FileSystemProvider.GetItemDetails(metadataFile);

            // get the value from cache
            if (_cacheEngine.Exists(key))
            {
                if (IsNotStale(_cacheEngine.GetAge(key), metadata.LastUpdated, item.LastUpdated))
                {
                    entity = EntityPersistance.Deserialize(_cacheEngine.Read(key));
                    if (entity.Version >= FirstCompatibleCache)
                    {
                        Statistics.GetInstance().Hit("cache.hit");
                        entity.Path = item.FullPath;
                        return entity;
                    }
                }
                // if it's not the latest version of the entity, delete it 
                _cacheEngine.Delete(key);
                Statistics.GetInstance().Hit("cache.expiry");
            }

            Statistics.GetInstance().Hit("factory.hit");
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") [metadata " + metadataFile + " : " + !String.IsNullOrEmpty(metadata.Name) + "]", "start");
#endif

            if (!String.IsNullOrEmpty(metadata.Name))
            {
                string metadataText = File.ReadAllText(metadataFile);
                entity = EntityPersistance.Deserialize(metadataText);
                if (String.IsNullOrEmpty(entity.Title)) { entity.Title = item.Name; }
                if (!String.IsNullOrEmpty(entity.BackgroundPath)) { entity.BackgroundPath = entity.BackgroundPath; }
                if (!String.IsNullOrEmpty(entity.IconPath)) { entity.IconPath = entity.IconPath; }

                entity.Path = item.FullPath;
                return entity;
            }

            EntityKind kind = DetermineKind(item);
            switch (kind)
            {
                case EntityKind.Album:
                    {
                        entity = new Album();
                        break;
                    }
                case EntityKind.Artist:
                    {
                        entity = new Artist();
                        break;
                    }
                case EntityKind.Folder:
                    {
                        entity = new Folder();
                        break;
                    }
                case EntityKind.Home:
                    {
                        entity = new Home();
                        break;
                    }
                case EntityKind.Playlist:
                    {
                        entity = new Playlist();
                        break;
                    }
                case EntityKind.Song:
                    {
                        entity = new Song();
                        break;
                    }
                case EntityKind.Genre:
                    {
                        entity = new Genre();
                        break;
                    }
                default:
                    {
                        entity = new Unknown {Path = item.FullPath};
                        break;
                    }
            }

            entity.Title = item.Name;
            entity.Path = item.FullPath;

            // do this here so that if the user browses to a folder that isn't cached, it retrieves some basic metadata
            TagSharpMetadataProvider.FetchLite(entity);
            _cacheEngine.Update(key, EntityPersistance.Serialize(entity));

            return entity;
        }

        #endregion

        private static EntityKind DetermineKind(FileSystemItem entity)
        {
            if ((entity.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                IEnumerable<FileSystemItem> content = FileSystemProvider.GetFolderContents(entity.FullPath);
                bool containsAlbums = false;
                bool containsFolders = false;

                foreach (FileSystemItem item in content)
                {
                    if (Util.Helper.IsSong(item.Name))
                    {
                        return EntityKind.Album;
                    }
                    if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        containsFolders = true;
                        containsAlbums = false;

                        IEnumerable<FileSystemItem> subItems = FileSystemProvider.GetFolderContents(item.FullPath);

                        if (subItems.Count() == 0)
                        {
                            return EntityKind.Folder;
                        }

                        foreach (FileSystemItem subItem in subItems)
                        {
                            if (Util.Helper.IsSong(subItem.FullPath))
                            {
                                containsAlbums = true;
                                continue;
                            }
                            if ((subItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                if (subItem.Name.StartsWith("disc ") || subItem.Name.StartsWith("CD"))
                                {
                                    containsAlbums = true;
                                    continue;
                                }
                            }
                        }
                    }
                }
                if (containsAlbums)
                {
                    return EntityKind.Artist;
                }
                if (containsFolders)
                {
                    return EntityKind.Genre;
                }
                return EntityKind.Folder;
            }
            if (Util.Helper.IsSong(entity.FullPath))
            {
                return EntityKind.Song;
            }
            if (Util.Helper.IsPlaylist(entity.FullPath))
            {
                return EntityKind.Playlist;
            }
            Logging.Logger.Info("unable to determine entity type for : " + entity.FullPath);
            return EntityKind.Unknown;
        }
    }
}