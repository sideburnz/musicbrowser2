using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using System.Text.RegularExpressions;

namespace MusicBrowser.Entities
{
    public static class EntityFactory
    {
        private static ICacheEngine _cacheEngine = CacheEngineFactory.GetEngine();
        private static InMemoryCache _MemCache = InMemoryCache.GetInstance();

        private enum EntityKind
        {
            Album = 101,
            Artist = 102,
            Folder = 103,
            Playlist = 105,
            Track = 106,
            Genre = 107,

            Video = 201,
            Episode = 202,
            Movie = 203,
            Season = 204,
            Show = 205,

            Photo = 301,
            PhotoAlbum = 302
        }

        public static baseEntity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static baseEntity GetItem(FileSystemItem item)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            string key = Util.Helper.GetCacheKey(item.FullPath);
            baseEntity entity;

            #region InMemoryCache
            // get from the Mem cache if it's cached there, this is the fastest cache
            entity = _MemCache.Fetch(key);
            if (entity != null)
            {
                return entity;
            }
            #endregion

            #region persistent cache
            // get the value from persistent cache
            entity = _cacheEngine.Fetch(key);
            if (entity != null && entity.CreateDate > item.LastUpdated)
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

            //switch (kind)
            //{
            //    case EntityKind.Album:
            //        entity = Factorize<Album>(item); break;

            //    case EntityKind.Track:
            //        entity = Factorize<Track>(item);
            //        TagSharpMetadataProvider.FetchLite(entity);
            //        break;
            //}

            _cacheEngine.Update(entity);
            _MemCache.Update(entity);
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.KindName + " - first principles", "finish");
#endif
            return entity;
        }

        // use some magic to create the entity and set some initial values
        private static T Factorize<T>(FileSystemItem item) where T : baseEntity, new()
        {
            T e = new T();
            e.Title = item.Name;
            e.Path = item.FullPath;
            e.CreateDate = item.Created;
            return e;
        }

        private static IEnumerable<string> nonphotoimages = new string[] { 
            "folder", 
            "banner", 
            "disc",
            "cover",
            "front",
            "back",
            "backdrop", 
            "backdrop1", 
            "backdrop2",
            "backdrop3",
            "backdrop4",
            "backdrop5",
            "backdrop6",
            "backdrop7",
            "backdrop8",
            "backdrop9",
        };

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
                        bool hasImages = false;
                        foreach (FileSystemItem item in items)
                        {
                            switch (item.Name.ToLower())
                            {
                                case "series.xml":
                                    return EntityKind.Show;
                                case "video_ts":
                                    if (IsEpisode(entity.Name))
                                    {
                                        return EntityKind.Episode;
                                    }
                                    return EntityKind.Movie;
                                case "mymovies.xml":
                                    return EntityKind.Movie;
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
                                    hasImages = true; break;
                                case EntityKind.Episode:
                                    return EntityKind.Season;
                                case EntityKind.Season:
                                    return EntityKind.Show;
                            }
                        }
                        if (hasImages) { return EntityKind.PhotoAlbum; }
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
                        if (IsEpisode(entity.Name))
                        {
                            return EntityKind.Episode;
                        }
                        return EntityKind.Movie;
                    }
                case Helper.knownType.Image:
                    {
                        // images have exceptions
                        if (!nonphotoimages.Contains(Path.GetFileNameWithoutExtension(entity.Name)))
                        {
                            return EntityKind.Photo;
                        }
                        break;
                    }
            }
            return null;
        }

        private static Regex _EpisodeRegEx = new Regex(@"^[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<epnumber>\d{1,3})");

        private static bool IsEpisode(string path)
        {
            return _EpisodeRegEx.Match(path).Success;
        }
    }
}