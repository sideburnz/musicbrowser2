using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.CacheEngine;

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
            // get all of the songs from sub folders

            foreach (string path in paths)
            {
#if DEBUG
                Logging.Logger.Verbose("PlaylistProvider.CreatePlaylist(" + path + ", " + queue + ", " + shuffle + ")", "loop");
#endif
                foreach (FileSystemItem item in FileSystemProvider.GetAllSubPaths(path))
                {
                    if (Util.Helper.IsSong(item.FullPath))
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
                    if (_entity.Kind.Equals(EntityKind.Song) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlaySong(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, false);
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
                        CreatePlaylist(paths, true, false);
                    }
                    break;
                case "cmdshuffle":
                    if (_entity.Kind.Equals(EntityKind.Song) || _entity.Kind.Equals(EntityKind.Playlist))
                    {
                        MediaCentre.Playlist.PlaySong(_entity, false);
                    }
                    else
                    {
                        CreatePlaylist(paths, false, true);
                    }
                    break;
                case "cmdfavourited":
                case "cmdmostplayed":
                case "cmdnew":
                    {
                        //TODO: write a GetIntSetting that accepts a min val to ensure this is safe
                        //TODO: put into a method
                        int size = Int32.Parse(Util.Config.GetInstance().GetSetting("AutoPlaylistSize"));
                        List<string> tracks = new List<string>();
                        switch (_action)
                        {
                            case "cmdfavourited":
                                tracks.AddRange(NearLineCache.GetInstance().FindFavorites()); break;
                            case "cmdmostplayed":
                                tracks.AddRange(NearLineCache.GetInstance().FindMostPlayed(size)); break;
                            case "cmdnew":
                                tracks.AddRange(NearLineCache.GetInstance().FindRecentlyAdded(size)); break;
                        }
                        //dedupe the list
                        tracks = tracks.Distinct().ToList();
                        MediaCentre.Playlist.PlayTrackList(tracks, false);
                        break;
                    }
            }
        }
        #endregion
    }
}
