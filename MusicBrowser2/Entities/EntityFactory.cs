﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.CacheEngine;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    public static class EntityFactory
    {
        private static ICacheEngine _cacheEngine = CacheEngine.CacheEngineFactory.GetCacheEngine();
        private static InMemoryCache _MemCache = InMemoryCache.GetInstance();

        public static Entity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static Entity GetItem(string item, bool forced)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item), forced);
        }

        public static Entity GetItem(FileSystemItem item)
        {
            return GetItem(item, false);
        }

        public static Entity GetItem(FileSystemItem item, bool forced)
        {
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            Entity entity;

            if (!forced)
            {
                #region InMemoryCache
                // get from the Mem cache if it's cached there, this is the fastest cache
                entity = _MemCache.Fetch(key);
                if (entity != null && entity.CacheDate > item.LastUpdated)
                {
                    return entity;
                }
                #endregion

                #region persistent cache
                // get the value from persistent cache
                entity = EntityPersistance.Deserialize(_cacheEngine.Fetch(key));
                if (entity != null && entity.CacheDate > item.LastUpdated)
                {
                    _MemCache.Update(entity);
                    return entity;
                }
                else
                {
                    Statistics.Hit("factory.missorexpired");
                }
                #endregion
            }

            // don't waste time trying to determine a known not entity
            if (Util.Helper.getKnownType(item) == Helper.knownType.Other) { return null; }
            if (item.Name.ToLower() == "metadata") { return null; }

            Statistics.Hit("factory.hit");

            EntityKind? kind = DetermineKind(item);
            if (kind == null) { return null; }

            entity = new Entity()
            {
                Kind = kind.GetValueOrDefault(),
                Title = item.Name,
                Path = item.FullPath,
                Added = item.Created,
                CacheDate = DateTime.Now
            };

            // these are needed for aggregation calculations
            switch (kind)
            {
                case EntityKind.Album: { entity.AlbumCount = 1; break; }
                case EntityKind.Artist: { entity.ArtistCount = 1; break; }
                case EntityKind.Track: { entity.TrackCount = 1; break; }
            }

            // do this here because some of the providers need basic data about the tracks
            TagSharpMetadataProvider.FetchLite(entity);
            _cacheEngine.Update(key, EntityPersistance.Serialize(entity));
            _MemCache.Update(entity);
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.KindName + " - first principles", "finish");
#endif
            return entity;
        }

        private static Nullable<EntityKind> DetermineKind(FileSystemItem entity)
        {
            Helper.knownType type = Helper.getKnownType(entity);

            switch (type)
            {
                case Helper.knownType.Folder:
                    {
                        // ignore metadata folders
                        if (entity.Name.ToLower() == "metadata") { return null; }

                        IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(entity.FullPath);
                        foreach (FileSystemItem item in items)
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
                case Helper.knownType.Track:
                    {
                        return EntityKind.Track;
                    }
                case Helper.knownType.Playlist:
                    {
                        return EntityKind.Playlist;
                    }
            }
            return null;
        }


        // works out where the metadata file is (if there is one)
        //private static string MetadataPath(FileSystemItem item)
        //{
        //    string metadataPath = Directory.GetParent(item.FullPath).FullName;

        //    string metadataLocal = metadataPath + "\\" + item.Name + "\\metadata.xml";
        //    if (File.Exists(metadataLocal))
        //    {
        //        return metadataLocal;
        //    }
        //    string metadataInParent = metadataPath + "\\metadata\\" + item.Name + ".xml";
        //    if (File.Exists(metadataInParent))
        //    {
        //        return metadataInParent;
        //    }
        //    return string.Empty;
        //}
    }
}