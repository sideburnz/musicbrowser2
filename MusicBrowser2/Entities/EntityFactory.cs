using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Entities.Kinds;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Entities
{
    public class EntityFactory : IEntityFactory
    {
        private static long FIRST_COMPATIBLE_CACHE = Util.Helper.ParseVersion("2.2.1.7");
        private IEntityCache _cache;

        #region IEntityFactory Members

        public void setCache(IEntityCache cache) { _cache = cache; }

        public IEntity getItem(string item)
        {
            return getItem(FileSystemProvider.getItemDetails(item));
        }

        public IEntity getItem(FileSystemItem item)
        {
            // don't waste time trying to determine a known not entity
            if (Util.Helper.IsNotEntity(item.FullPath)) { return new Unknown(); }
            if (item.Name.ToLower() == "metadata") { return new Unknown(); }


            IEntity entity;
            string metadataPath = Directory.GetParent(item.FullPath) + "\\metadata\\";
            string metadataFile = metadataPath + item.Name + ".xml";
            string key = Util.Helper.GetCacheKey(item.FullPath);
            FileSystemItem metadata = FileSystemProvider.getItemDetails(metadataFile);

#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") [metadata " + metadataFile + " : " + !String.IsNullOrEmpty(metadata.Name) + "]", "start");
#endif
            // get the value from cache
            if (_cache.Exists(key))
            {
                if (_cache.IsValid(key, metadata.LastUpdated, item.LastUpdated))
                {
                    entity = _cache.Read(key);
                    if (entity.Version >= FIRST_COMPATIBLE_CACHE)
                    {
                        entity.Path = item.FullPath;
                        return entity;
                    }
                    entity = null;
                }
                // if it's not the latest version of the entity, delete it 
                _cache.Delete(key);
            }

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
                        entity = new Unknown();
                        entity.Path = item.FullPath;
                        return entity;
                    }
            }

            entity.Title = item.Name;
            entity.Path = item.FullPath;

            // do this here so that if the user browses to a folder that isn't cached, it retrieves
            if (entity.Kind.Equals(EntityKind.Song))
            {
                CommonTaskQueue.Enqueue(new TagSharpMetadataProvider(entity, _cache), true);
            }

            entity.CalculateValues();
            _cache.Update(key, entity);

            return entity;
        }

        #endregion

        private static EntityKind DetermineKind(FileSystemItem entity)
        {
            if (Util.Helper.IsSong(entity.FullPath))
            {
                return EntityKind.Song;
            }
            if (Util.Helper.IsPlaylist(entity.FullPath))
            {
                return EntityKind.Playlist;
            }
            if ((entity.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                IEnumerable<FileSystemItem> content = FileSystemProvider.GetFolderContents(entity.FullPath);
                bool containsSongs = false;
                bool containsFolders = false;

                foreach (FileSystemItem item in content)
                {
                    if (Util.Helper.IsSong(item.Name)) { containsSongs = true; break; }
                    if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        containsFolders = true;
                    }
                }
                // if there's tracks in the folder then it's an album
                if (containsSongs)
                {
                    return EntityKind.Album;
                }
                if (containsFolders)
                {
                    return EntityKind.Artist;
                }
                return EntityKind.Folder;
            }
            Logging.Logger.Info("unable to determine entity type for : " + entity.FullPath);
            return EntityKind.Unknown;
        }
    }
}