using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata.Lite;
using MusicBrowser.Util;
using System.Text.RegularExpressions;
using System.Xml;

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
            Show = 205
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

            // don't waste time trying to determine a known not entity
            if (item.Name.ToLower() == "metadata") { return null; }
            if (Util.Helper.getKnownType(item) == Helper.knownType.Other) { return null; }

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
            if (entity != null && entity.TimeStamp > item.LastUpdated)
            {
                _MemCache.Update(entity);
                return entity;
            }
            else
            {
                Statistics.Hit("factory.missorexpired");
            }
            #endregion

            Engines.Logging.LoggerEngineFactory.Debug("Manufacturing " + item.FullPath);

            Statistics.Hit("factory.hit");

            EntityKind? kind = DetermineKind(item);
            if (kind == null) { return null; }

            switch (kind)
            {
                case EntityKind.Album:
                    entity = Factorize<Album>(item); break;
                case EntityKind.Artist:
                    entity = Factorize<Artist>(item); break;
                case EntityKind.Folder:
                    entity = Factorize<Folder>(item); break;
                case EntityKind.Genre:
                    entity = Factorize<Genre>(item); break;
                case EntityKind.Season:
                    entity = Factorize<Season>(item); break;
                case EntityKind.Show:
                    entity = Factorize<Show>(item); break;
                case EntityKind.Playlist:
                    entity = Factorize<Playlist>(item); break;
                case EntityKind.Track:
                    entity = Factorize<Track>(item);
                    MediaInfoProvider.FetchLite(entity);
                    break;
                case EntityKind.Episode:
                    entity = Factorize<Episode>(item);
                    VideoFilenameMetadataProvider.FetchLite(entity);
                    MediaInfoProvider.FetchLite(entity);
                    break;
                case EntityKind.Movie:
                    entity = Factorize<Movie>(item);
                    MediaInfoProvider.FetchLite(entity);
                    break;
            }

            entity.UpdateCache();
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.Kind + " - first principles", "finish");
#endif
            return entity;
        }

        // use some magic to create the entity and set some initial values
        private static T Factorize<T>(FileSystemItem item) where T : baseEntity, new()
        {
            T e = new T();
            if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                e.Title = item.Name;
            }
            else
            {
                e.Title = Path.GetFileNameWithoutExtension(item.Name);
            }
            e.Path = item.FullPath;
            e.LastUpdated = item.LastUpdated;
            e.TimeStamp = DateTime.Now;
            if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                e.ThumbPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Thumb);
                e.BannerPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Banner);
                e.LogoPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Logo);
                e.BackgroundPaths = ImageProvider.LocateBackdropList(item.FullPath);
            }
            return e;
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

                        int movies = 0;

                        IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(entity.FullPath);
                        foreach (FileSystemItem item in items)
                        {
                            switch (item.Name.ToLower())
                            {
                                case "series.xml":
                                    return EntityKind.Show;
                                case "movie.xml":
                                    return EntityKind.Movie;
                                case "genre.xml":
                                    return EntityKind.Genre;
                                case "album.xml":
                                    return EntityKind.Album;
                                case "artist.xml":
                                    return EntityKind.Artist;
                                case "season.xml":
                                    return EntityKind.Season;
                                case "video_ts":
                                    if (IsEpisode(entity.Name))
                                    {
                                        return EntityKind.Episode;
                                    }
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
                                case EntityKind.Episode:
                                    return EntityKind.Season;
                                case EntityKind.Season:
                                    return EntityKind.Show;
                                case EntityKind.Movie:
                                    if ((item.Attributes & FileAttributes.Directory) != FileAttributes.Directory) { movies++; }
                                    break;
                            }
                        }

                        if (movies > 0 && movies < 3) { return EntityKind.Movie; }
                        //TODO: introduce a "movie series" type
                        //if (movies > 0) { return EntityKind.Folder; }
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
            }
            return null;
        }

        private static Regex _EpisodeRegEx = new Regex(@"^[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<epnumber>\d{1,3})");

        private static bool IsEpisode(string path)
        {
            return _EpisodeRegEx.Match(path).Success;
        }

        // works out where the metadata file is (if there is one)
        //private static string MetadataPath(string item)
        //{
        //    string itemName = Path.GetFileNameWithoutExtension(item);
        //    string metadataPath = Directory.GetParent(item).FullName;
        //    FileSystemItem metadataFile;

        //    string metadataLocal = metadataPath + "\\" + itemName + "\\metadata.xml";
        //    metadataFile = FileSystemProvider.GetItemDetails(metadataLocal);
        //    if (!String.IsNullOrEmpty(metadataFile.Name))
        //    {
        //        return metadataFile.FullPath;
        //    }
        //    string metadataInParent = metadataPath + "\\metadata\\" + itemName + ".xml";
        //    metadataFile = FileSystemProvider.GetItemDetails(metadataInParent);
        //    // this either returns detail or an empty struct which would indicate not found
        //    if (!String.IsNullOrEmpty(metadataFile.Name))
        //    {
        //        return metadataFile.FullPath;
        //    }
        //    return String.Empty;
        //}

    }
}