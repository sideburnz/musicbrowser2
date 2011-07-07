using System;
using System.IO;
using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities.Kinds;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Providers.Background;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Entities
{
    public class EntityFactory
    {
        private static readonly long FirstCompatibleCache = Util.Helper.ParseVersion("2.2.1.7");
        private ICacheEngine _cacheEngine = CacheEngine.CacheEngineFactory.GetCacheEngine();

        #region IEntityFactory Members

        public IEntity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public IEntity GetItem(FileSystemItem item)
        {
            // don't waste time trying to determine a known not entity
            if (Util.Helper.IsNotEntity(item.FullPath)) { return new Unknown(); }
            if (item.Name.ToLower() == "metadata") { return new Unknown(); }

            IEntity entity;
            string metadataPath = Directory.GetParent(item.FullPath) + "\\metadata\\";
            string metadataFile = metadataPath + item.Name + ".xml";
            string key = Util.Helper.GetCacheKey(item.FullPath);
            FileSystemItem metadata = FileSystemProvider.GetItemDetails(metadataFile);


            // get the value from cache
            if (_cacheEngine.Exists(key))
            {
                if (_cacheEngine.IsValid(key, metadata.LastUpdated, item.LastUpdated))
                {
                    entity = EntityPersistance.Deserialize(_cacheEngine.Read(key));
                    if (entity.Version >= FirstCompatibleCache)
                    {
                        entity.Path = item.FullPath;
                        return entity;
                    }
                }
                // if it's not the latest version of the entity, delete it 
                _cacheEngine.Delete(key);
            }

            Statistics.GetInstance().Hit("factory.hit");
#if DEBUG
            Logging.LoggerFactory.Verbose("Factory.getItem(" + item.FullPath + ") [metadata " + metadataFile + " : " + !String.IsNullOrEmpty(metadata.Name) + "]", "start");
#endif

            if (!String.IsNullOrEmpty(metadata.Name))
            {
                string metadataText = File.ReadAllText(metadataFile);
                entity = EntityPersistance.Deserialize(metadataText);
                if (String.IsNullOrEmpty(entity.Title)) { entity.Title = item.Name; }
                if (!String.IsNullOrEmpty(entity.BackgroundPath)) { entity.BackgroundPath = Path.Combine(metadataPath, entity.BackgroundPath); }
                if (!String.IsNullOrEmpty(entity.IconPath)) { entity.IconPath = Path.Combine(metadataPath, entity.IconPath); }

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
                default:
                    {
                        // unknowns skip the caching etc
                        entity = new Unknown {Path = item.FullPath};
                        return entity;
                    }
            }

            entity.Title = item.Name;
            entity.Path = item.FullPath;

            // do this here so that if the user browses to a folder that isn't cached, it retrieves
            if (entity.Kind.Equals(EntityKind.Song))
            {
                CommonTaskQueue.Enqueue(new TagSharpMetadataProvider(entity), true);
            }

            entity.CalculateValues();
            _cacheEngine.Update(key, EntityPersistance.Serialize(entity));

            return entity;
        }

        #endregion

        private static EntityKind DetermineKind(FileSystemItem entity)
        {
            if ((entity.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                IEnumerable<FileSystemItem> content = FileSystemProvider.GetFolderContents(entity.FullPath);
                bool containsFolders = false;

                foreach (FileSystemItem item in content)
                {
                    if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        containsFolders = true;
                    }
                    if (Util.Helper.IsSong(item.Name)) 
                    {
                        return EntityKind.Album; 
                    }
                }
                if (containsFolders)
                {
                    return EntityKind.Artist;
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
            Logging.LoggerFactory.Info("unable to determine entity type for : " + entity.FullPath);
            return EntityKind.Unknown;
        }
    }
}