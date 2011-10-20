using System;
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

        // this resets the entity back to a near-clean slate
        public static void Refactor(Entity entity)
        {
            entity.BackgroundPaths = new List<string>();
            entity.IconPath = String.Empty;

            entity.AlbumArtist = String.Empty;
            entity.AlbumName = String.Empty;
            entity.ArtistName = String.Empty;
            entity.Channels = String.Empty;
            entity.Codec = String.Empty;
            entity.DiscId = String.Empty;
            entity.DiscNumber = 0;
            entity.Duration = 0;
            entity.Favorite = false;
            entity.Genre = String.Empty;
            entity.Label = String.Empty;
            entity.Listeners = 0;
            entity.Lyrics = String.Empty;
            entity.MusicBrainzID = String.Empty;
            entity.Performers = new List<string>();
            entity.PlayCount = 0;
            entity.ProviderTimeStamps = new Dictionary<string, DateTime>();
            entity.Rating = 0;
            entity.ReleaseDate = DateTime.MinValue;
            entity.Resolution = String.Empty;
            entity.SampleRate = String.Empty;
            entity.Summary = String.Empty;
            entity.TotalPlays = 0;
            entity.TrackCount = 0;
            entity.TrackName = String.Empty;
            entity.TrackNumber = 0;

            //force it to refresh
            string path = entity.Path;
            entity.Path = String.Empty;
            entity.Path = path;

            switch (entity.Kind)
            {
                case EntityKind.Album: { entity.AlbumCount = 1; break; }
                case EntityKind.Artist: { entity.ArtistCount = 1; break; }
                case EntityKind.Track: { entity.TrackCount = 1; break; }
            }

            // do this here because some of the providers need basic data about the tracks
            TagSharpMetadataProvider.FetchLite(entity);
        }

        public static Entity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static Entity GetItem(FileSystemItem item)
        {
#if DEBUG
            Logging.Logger.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            FileSystemItem metadataFile = MetadataPath(item);
            Entity entity;

            #region InMemoryCache
            // get from the Mem cache if it's cached there, this is the fastest cache
            entity = _MemCache.Fetch(key);
            if (entity != null && entity.CacheDate > item.LastUpdated && entity.CacheDate > metadataFile.LastUpdated)
            {
                return entity;
            }
            #endregion

            #region persistent cache
            // get the value from persistent cache
            entity = EntityPersistance.Deserialize(_cacheEngine.Fetch(key));
            if (entity != null && entity.CacheDate > item.LastUpdated && entity.CacheDate > metadataFile.LastUpdated)
            {
                _MemCache.Update(entity);
                return entity;
            }
            else
            {
                Statistics.Hit("factory.missorexpired");
            }
            #endregion


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

            //TODO: apply data from the metadata file, if one exists
            if (!String.IsNullOrEmpty(metadataFile.Name))
            {

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
        private static FileSystemItem MetadataPath(FileSystemItem item)
        {
            string metadataPath = Directory.GetParent(item.FullPath).FullName;
            FileSystemItem metadataFile;

            string metadataLocal = metadataPath + "\\" + item.Name + "\\metadata.xml";
            metadataFile = FileSystemProvider.GetItemDetails(metadataLocal);
            if (!String.IsNullOrEmpty(metadataFile.Name))
            {
                return metadataFile;
            }
            string metadataInParent = metadataPath + "\\metadata\\" + item.Name + ".xml";
            metadataFile = FileSystemProvider.GetItemDetails(metadataInParent);
            // this either returns detail or an empty struct which would indicate not found
            return metadataFile;
        }
    }
}