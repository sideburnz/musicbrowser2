using System.Collections.Generic;
using System.IO;
using MusicBrowser.Providers;

namespace MusicBrowser.Util
{
    public static class EntityResolver
    {
        // all recognised entity types
        public enum EntityKind
        {
            Album = 101,
            Artist = 102,
            Folder = 103,
            Playlist = 105,
            Track = 106,
            Genre = 107,
            Episode = 202,
            Movie = 203,
            Season = 204,
            Show = 205
        }

        private static readonly int MaxMovieParts = Config.GetInstance().GetIntSetting("PlaylistLimit");
        private static readonly bool AllowMoviePlaylists = Config.GetInstance().GetBooleanSetting("EnableMoviePlaylists");
        private static readonly Dictionary<FileSystemItem, EntityKind?> EntityResolverCache = new Dictionary<FileSystemItem, EntityKind?>();

        // We wrap the old resolver in a method that handles caching because we normally resolve a
        // shed load of items at a time and because we resolve child items to work out parent items
        // some items will end up being resolved multiple times in an execution
        public static EntityKind? Resolve(FileSystemItem entity)
        {
            // check if we've resolved this entity this execution
            if (EntityResolverCache.ContainsKey(entity))
            {
                return EntityResolverCache[entity];
            }

            // resolve it
            EntityKind? kind = InternalResolve(entity);

            // cache it
            EntityResolverCache.Add(entity, kind);

            // return it
            return kind;
        }


        private static EntityKind? InternalResolve(FileSystemItem entity)
        {
            Helper.KnownType type = Helper.GetKnownType(entity);

            switch (type)
            {
                case Helper.KnownType.Folder:
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
                                    if (Helper.IsEpisode(entity.Name))
                                    {
                                        return EntityKind.Episode;
                                    }
                                    return EntityKind.Movie;
                            }

                            EntityKind? e = Resolve(item);
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

                        // assimilates multiple movie files into a single movie, if the user wants it
                        if (AllowMoviePlaylists)
                        {
                            if (movies > 0 && movies <= MaxMovieParts)
                            {
                                return EntityKind.Movie;
                            }
                        }

                        return EntityKind.Folder;
                    }
                case Helper.KnownType.Track:
                    {
                        return EntityKind.Track;
                    }
                case Helper.KnownType.Playlist:
                    {
                        return EntityKind.Playlist;
                    }
                case Helper.KnownType.Video:
                    {
                        if (Helper.IsEpisode(entity.Name))
                        {
                            return EntityKind.Episode;
                        }
                        return EntityKind.Movie;
                    }
            }
            return null;
        }
    }
}
