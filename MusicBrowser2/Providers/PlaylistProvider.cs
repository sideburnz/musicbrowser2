using System;
using System.Collections.Generic;
using System.Threading;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Providers
{
    class PlaylistProvider : IBackgroundTaskable
    {
        private readonly string _action;
        private readonly IEntity _entity;
        private static readonly Random Random = new Random();

        public PlaylistProvider(string action, IEntity entity)
        {
            _action = action.ToLower();
            _entity = entity;
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

        private static void CreatePlaylist(IEnumerable<string> paths, bool queue, bool shuffle, bool favorites, int minPlays, int minStars)
        {
            EntityFactory factory = new EntityFactory();
            List<string> tracks = new List<string>();
            // get all of the songs from sub folders

            foreach (string path in paths)
            {
#if DEBUG
                Logging.Logger.Verbose("PlaylistProvider.CreatePlaylist(" + path + ", " + queue + ", " + shuffle + ", " + favorites + ", " + minPlays + ", " + minStars + ")", "loop");
#endif
                foreach (FileSystemItem item in FileSystemProvider.GetAllSubPaths(path))
                {
                    if (Util.Helper.IsSong(item.FullPath))
                    {
                        // fairly rough implementation to play favorite tracks by various criteria
                        if (favorites || minPlays > 0 || minStars > 0)
                        {
                            IEntity e = factory.GetItem(item); // this is very very very slow to do in bulk
                            if (favorites && e.Favorite)
                            {
                                tracks.Add(item.FullPath);
                                continue;
                            }
                            if (minPlays > 0 && e.PlayCount >= minPlays)
                            {
                                tracks.Add(item.FullPath);
                                continue;
                            }
                            if (minStars > 0 && e.Rating >= minStars)
                            {
                                tracks.Add(item.FullPath);
                                continue;
                            }
                        }
                        else
                        {
                            tracks.Add(item.FullPath);
                        }
                    }
                }
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
                paths = Entities.Kinds.Home.Paths; 
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
                    if (_entity.Kind.Equals(EntityKind.Song) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlaySong(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, false, false, 0, 0);
                    }
                    break;
                case "cmdaddtoqueue":
                    if (_entity.Kind.Equals(EntityKind.Song) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        Models.UINotifier.GetInstance().Message = "adding \"" + _entity.Title + "\" to playlist";
                        MediaCentre.Playlist.PlaySong(_entity, true);
                    }
                    else
                    {
                        CreatePlaylist(paths, true, false, false, 0, 0);
                    }
                    break;
                case "cmdshuffle":
                    if (_entity.Kind.Equals(EntityKind.Song) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlaySong(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, true, false, 0, 0);
                    }
                    break;
                case "cmdplayfavorites":
                    {
                        CreatePlaylist(paths, false, false, true, 0, 5);
                        break;
                    }
            }
        }
        #endregion
    }
}
