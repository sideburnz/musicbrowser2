using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    public static class EntityFactory
    {
        private static ICacheEngine _cacheEngine = CacheEngineFactory.GetEngine();
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

            EntityKind? kind = DetermineKind(FileSystemProvider.GetItemDetails(path));
            entity.Kind = kind.GetValueOrDefault();
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Determined " + path + " to be a " + kind.ToString(), "start");
#endif

            switch (entity.Kind)
            {
                case EntityKind.Album: { entity.AlbumCount = 1; break; }
                case EntityKind.Artist: { entity.ArtistCount = 1; break; }
                case EntityKind.Track: { entity.TrackCount = 1; break; }
            }

            // do this here because some of the providers need basic data about the tracks
            TagSharpMetadataProvider.FetchLite(entity);
        }

        //public static Entity HandleExperimentalFeatures(Entity entity)
        //{
        //    if ((entity.Kind == EntityKind.Photo || entity.Kind == EntityKind.PhotoAlbum) && !Config.GetInstance().GetBooleanSetting("EnableExperimentalPhotoSupport"))
        //    {
        //        return null;
        //    }
        //    if ((entity.Kind == EntityKind.DVD || entity.Kind == EntityKind.Episode || entity.Kind == EntityKind.Movie || entity.Kind == EntityKind.Video) && !Config.GetInstance().GetBooleanSetting("EnableExperimentalVideoSupport"))
        //    {
        //        return null;
        //    }
        //    return entity;
        //}

        public static Entity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static Entity GetItem(FileSystemItem item)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            Entity entity;

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
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.KindName + " - first principles", "finish");
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
                            if (item.Name.ToLower() == "video_ts")
                            {
                                return EntityKind.Video;
                            }
                            EntityKind? e = DetermineKind(item);
                            switch (e)
                            {
                                case EntityKind.Track:
                                    return EntityKind.Album;
                                case EntityKind.Album:
                                    return EntityKind.Artist;
                                case EntityKind.Artist:
                                    return EntityKind.Genre;
                                case EntityKind.Photo:
                                    return EntityKind.PhotoAlbum;
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
                case Helper.knownType.Video:
                    {
                        return EntityKind.Video;
                    }
                case Helper.knownType.Image:
                    {
                        // images have exception
                        string[] nonphotoimages = new string[] { "folder", "backdrop", "banner" };
                        if (!nonphotoimages.Contains(System.IO.Path.GetFileNameWithoutExtension(entity.Name)))
                        {
                            return EntityKind.Photo;
                        }
                        break;
                    }
            }
            return null;
        }

    }
}