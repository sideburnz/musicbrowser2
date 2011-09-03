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
        private static readonly long FirstCompatibleCache = Util.Helper.ParseVersion("2.2.2.6");
        private readonly ICacheEngine _cacheEngine = CacheEngineFactory.GetCacheEngine();

        #region IEntityFactory Members

        public IEntity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        bool IsFresh(DateTime cacheDate, params DateTime[] comparisons)
        {
            foreach (DateTime d in comparisons)
            {
                if (d >= cacheDate) { return false; }
            }
            return true;
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
            if (!Util.Helper.IsEntity(item.FullPath)) { return new Unknown(); }
            if (item.Name.ToLower() == "metadata") { return new Unknown(); }

#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            IEntity entity;

            #region NearLine Cache
            // get from the NL cache if it's cached there, this is the fastest cache but it's not persistent
            entity = NearLineCache.GetInstance().Fetch(key);
            if (entity.Kind != EntityKind.Unknown) 
            {
#if DEBUG
                Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") - NearLine cache", "end");
#endif
                entity.Path = item.FullPath;
                return entity;
            }
            #endregion

            #region persistent cache
            // get the value from persistent cache
            if (_cacheEngine.Exists(key))
            {
                if (IsFresh(_cacheEngine.GetAge(key), item.LastUpdated)) 
                {
                    entity = EntityPersistance.Deserialize(_cacheEngine.Read(key));
                    if (entity.Kind != EntityKind.Unknown)
                    {
#if DEBUG
                        Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") - persistent cache", "end");
#endif
                        NearLineCache.GetInstance().Update(entity);
                        Statistics.GetInstance().Hit("cache.hit");
                        entity.Path = item.FullPath;
                        return entity;
                    }
                }

                // if it's not the latest version of the entity, delete it refreshing the cache
                _cacheEngine.Delete(key);
                Statistics.GetInstance().Hit("cache.expiry");
            }
            #endregion

            Statistics.GetInstance().Hit("factory.hit");

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

            // do this here because some of the providers need basic data about the tracks
            TagSharpMetadataProvider.FetchLite(entity);
            _cacheEngine.Update(key, EntityPersistance.Serialize(entity));
            NearLineCache.GetInstance().Update(entity);
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.KindName + " - first principles", "finish");
#endif
            return entity;
        }

        #endregion

        private static EntityKind DetermineKind(FileSystemItem entity)
        {
            if (!Util.Helper.IsEntity(entity.FullPath))
            {
                return EntityKind.Unknown;
            }
            if (Util.Helper.IsSong(entity.FullPath))
            {
                return EntityKind.Song;
            }
            if (Util.Helper.IsFolder(entity.Attributes))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(entity.FullPath);

                foreach(FileSystemItem item in items)
                {
                    EntityKind e = DetermineKind(item);
                    switch (e)
                    {
                        case EntityKind.Song:
                            return EntityKind.Album;
                        case EntityKind.Album:
                            return EntityKind.Artist;
                        case EntityKind.Artist:
                            return EntityKind.Genre;
                    }
                }
                return EntityKind.Folder;
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