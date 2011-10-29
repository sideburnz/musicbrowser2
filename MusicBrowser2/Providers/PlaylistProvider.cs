using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Providers
{
    class PlaylistProvider : IBackgroundTaskable
    {
        private readonly string _action;
        private readonly Entity _entity;
        private static readonly Random Random = new Random();

        public PlaylistProvider(string action, Entity entity)
        {
            _action = action.ToLower();
            _entity = entity;
        }

        public IEnumerable<string> FindFavorites()
        {
            return InMemoryCache.GetInstance().DataSet
                .Where(item => ((item.Rating >= 90) || (item.Favorite)) && (item.Kind == EntityKind.Track))
                .Select(item => item.Path);
        }

        public IEnumerable<string> FindMostPlayed(int records)
        {
            return InMemoryCache.GetInstance().DataSet
                .Where(item => item.Kind == EntityKind.Track)
                .OrderByDescending(item => item.PlayCount)
                .Take(records)
                .Select(item => item.Path);
        }


        public IEnumerable<string> FindPopularOnLastFM(int records)
        {
            return InMemoryCache.GetInstance().DataSet
                .Where(item => item.Kind == EntityKind.Track)
                .OrderByDescending(item => item.TotalPlays)
                .Take(records)
                .Select(item => item.Path);
        }

        public IEnumerable<string> FindRecentlyAdded(int records)
        {
            return InMemoryCache.GetInstance().DataSet
                .Where(item => item.Kind == EntityKind.Track)
                .OrderByDescending(item => item.Added)
                .Take(records)
                .Select(item => item.Path);
        }

        public IEnumerable<string> FindRandomPlayed(int records, int sample)
        {
            return InMemoryCache.GetInstance().DataSet
                .Where(item => item.Kind == EntityKind.Track)
                .OrderByDescending(item => item.PlayCount)
                .Take(sample)
                .OrderBy(item => Guid.NewGuid())
                .Take(records)
                .Select(item => item.Path);
        }


        public static void ShuffleList<TE>(IList<TE> list)
        {
            if (list.Count > 1)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    TE tmp = list[i];
                    int randomIndex = Random.Next(i + 1);
                    //Swap elements
                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

        private static void CreatePlaylist(IEnumerable<string> paths, bool queue, bool shuffle)
        {
            List<string> tracks = new List<string>();
            // get all of the tracks from sub folders

            foreach (string path in paths)
            {
#if DEBUG
                Engines.Logging.LoggerEngineFactory.Verbose("PlaylistProvider.CreatePlaylist(" + path + ", " + queue + ", " + shuffle + ")", "loop");
#endif
                foreach (FileSystemItem item in FileSystemProvider.GetAllSubPaths(path))
                {
                    if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Track)
                    {
                        tracks.Add(item.FullPath);
                    }
                }

                //dedupe the list
                tracks = tracks.Distinct().ToList();
            }
            // shuffle the tracks if requested
            if (shuffle)
            {
                ShuffleList(tracks);
            }
            // Kick off a new thread
            MediaCentre.Playlist.PlayTrackList(tracks, queue);
        }

        #region IBackgroundTaskable Members
        public string Title
        {
            get { return GetType().ToString(); }
        }

        public void Execute()
        {
            IEnumerable<string> paths;
            if (_entity.Kind.Equals(EntityKind.Home)) 
            { 
                paths = Providers.FolderItems.HomePathProvider.Paths; 
            }
            else
            {
                List<string> pathList = new List<string>();
                pathList.Add(_entity.Path);
                paths = pathList;
            }

            switch (_action)
            {
                case "cmdplayall":
                    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlayTrack(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, false);
                    }
                    break;
                case "cmdaddtoqueue":
                    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        Models.UINotifier.GetInstance().Message = "adding \"" + _entity.Title + "\" to playlist";
                        MediaCentre.Playlist.PlayTrack(_entity, true);
                    }
                    else
                    {
                        CreatePlaylist(paths, true, false);
                    }
                    break;
                case "cmdshuffle":
                    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlayTrack(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, true);
                    }
                    break;
                case "cmdfavourited":
                case "cmdmostplayed":
                case "cmdnew":
                case "cmdlastfm":
                case "cmdrandom":
                    {
                        int size = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
                        List<string> tracks = new List<string>();
                        switch (_action)
                        {
                            case "cmdfavourited":
                                tracks.AddRange(FindFavorites()); break;
                            case "cmdmostplayed":
                                tracks.AddRange(FindMostPlayed(size)); break;
                            case "cmdnew":
                                tracks.AddRange(FindRecentlyAdded(size)); break;
                            case "cmdrandom":
                                tracks.AddRange(FindRandomPlayed(size, size * 5)); break;
                            case "cmdlastfm":
                                tracks.AddRange(FindPopularOnLastFM(size)); break;
                        }
                        //dedupe the list
                        MediaCentre.Playlist.PlayTrackList(tracks, false);
                        break;
                    }
            }
        }
        #endregion
    }
}
