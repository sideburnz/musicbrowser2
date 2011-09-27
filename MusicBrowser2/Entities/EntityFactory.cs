using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.CacheEngine;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;

namespace MusicBrowser.Entities
{
    public static class EntityFactory
    {
        private static ICacheEngine _cacheEngine = CacheEngine.CacheEngineFactory.GetCacheEngine();
        private static NearLineCache _NLCache = NearLineCache.GetInstance();

        public static Entity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static Entity GetItem(FileSystemItem item)
        {
            // don't waste time trying to determine a known not entity
            if (!Util.Helper.IsEntity(item.FullPath)) { return null; }
            if (item.Name.ToLower() == "metadata") { return null; }

#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            Entity entity;

            #region NearLine 
            // get from the NL cache if it's cached there, this is the fastest cache
            entity = _NLCache.Fetch(key);
            if (entity != null && IsFresh(entity.CacheDate, item.LastUpdated))
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
                    if (entity != null)
                    {
#if DEBUG
                        Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") - persistent cache", "end");
#endif
                        _NLCache.Update(entity);
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

            EntityKind? kind = DetermineKind(item);
            if (kind == null) { return null; }

            entity = new Entity();
            entity.Kind = kind.GetValueOrDefault();

            // these are needed for aggregation calculations
            switch (entity.Kind)
            {
                case EntityKind.Album: { entity.AlbumCount = 1; break; }
                case EntityKind.Artist: { entity.ArtistCount = 1; break; }
                case EntityKind.Track: { entity.TrackCount = 1; break; }
            }

            entity.Title = item.Name;
            entity.Path = item.FullPath;
            entity.Added = item.Created;

            // do this here because some of the providers need basic data about the tracks
            TagSharpMetadataProvider.FetchLite(entity);
            _cacheEngine.Update(key, EntityPersistance.Serialize(entity));
            _NLCache.Update(entity);
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.KindName + " - first principles", "finish");
#endif
            return entity;
        }

        private static Nullable<EntityKind> DetermineKind(FileSystemItem entity)
        {
            if (!Util.Helper.IsEntity(entity.FullPath))
            {
                return null;
            }
            if (Util.Helper.IsTrack(entity.FullPath))
            {
                return EntityKind.Track;
            }
            if (Util.Helper.IsFolder(entity.Attributes))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(entity.FullPath);

                foreach(FileSystemItem item in items)
                {
                    EntityKind? e = DetermineKind(item);
                    switch (e)
                    {
                        case EntityKind.Track:
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
            return null;
        }

        private static bool IsFresh(DateTime cacheDate, params DateTime[] comparisons)
        {
            foreach (DateTime d in comparisons)
            {
                if (d >= cacheDate) { return false; }
            }
            return true;
        }

        // works out where the metadata file is (if there is one)
        private static string MetadataPath(FileSystemItem item)
        {
            string metadataPath = Directory.GetParent(item.FullPath).FullName;

            string metadataLocal = metadataPath + "\\" + item.Name + "\\metadata.xml";
            if (File.Exists(metadataLocal))
            {
                return metadataLocal;
            }
            string metadataInParent = metadataPath + "\\metadata\\" + item.Name + ".xml";
            if (File.Exists(metadataInParent))
            {
                return metadataInParent;
            }
            return string.Empty;
        }
    }
}