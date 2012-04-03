//using System.Collections.Generic;
//using System.Linq;
//using MusicBrowser.Entities;
//using MusicBrowser.Providers.Background;

//namespace MusicBrowser.Providers
//{
//    class PlaylistProvider : IBackgroundTaskable
//    {
//        private readonly string _action;
//        private readonly baseEntity _entity;

//        public PlaylistProvider(string action, baseEntity entity)
//        {
//            _action = action.ToLower();
//            _entity = entity;
//        }

//        //public IEnumerable<string> FindFavorites()
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => ((item.Rating >= 90) || (item.Favorite)) && (item.Kind == EntityKind.Track))
//        //        .Select(item => item.Path);
//        //}

//        //public IEnumerable<string> FindMostPlayed(int records)
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => item.Kind == EntityKind.Track)
//        //        .OrderByDescending(item => item.PlayCount)
//        //        .Take(records)
//        //        .Select(item => item.Path);
//        //}

//        //public IEnumerable<string> FindPopularOnLastFM(int records)
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => item.Kind == EntityKind.Track)
//        //        .OrderByDescending(item => item.TotalPlays)
//        //        .Take(records)
//        //        .Select(item => item.Path);
//        //}

//        //public IEnumerable<string> FindRandomPopularOnLastFM(int records, int sample)
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => item.Kind == EntityKind.Track)
//        //        .OrderByDescending(item => item.TotalPlays)
//        //        .Take(sample)
//        //        .OrderBy(item => Guid.NewGuid())
//        //        .Take(records)
//        //        .Select(item => item.Path);
//        //}

//        //public IEnumerable<string> FindRecentlyAdded(int records)
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => item.Kind == EntityKind.Track)
//        //        .OrderByDescending(item => item.Added)
//        //        .Take(records)
//        //        .Select(item => item.Path);
//        //}

//        //public IEnumerable<string> FindRandomPlayed(int records, int sample)
//        //{
//        //    return InMemoryCache.GetInstance().DataSet
//        //        .Where(item => item.Kind == EntityKind.Track)
//        //        .OrderByDescending(item => item.PlayCount)
//        //        .Take(sample)
//        //        .OrderBy(item => Guid.NewGuid())
//        //        .Take(records)
//        //        .Select(item => item.Path);
//        //}


//        private static void CreatePlaylist(IEnumerable<string> paths, bool queue, bool shuffle)
//        {
//            List<string> tracks = new List<string>();
//            // get all of the tracks from sub folders

//            foreach (string path in paths)
//            {
//#if DEBUG
//                Engines.Logging.LoggerEngineFactory.Verbose("PlaylistProvider.CreatePlaylist(" + path + ", " + queue + ", " + shuffle + ")", "loop");
//#endif
//                tracks.AddRange(from item in FileSystemProvider.GetAllSubPaths(path) where Util.Helper.GetKnownType(item) == Util.Helper.KnownType.Track select item.FullPath);

//                //dedupe the list
//                tracks = tracks.Distinct().ToList();
//            }
//            // shuffle the tracks if requested
//            if (shuffle)
//            {
//                Util.Helper.Shuffle(tracks);
//            }
//            // Kick off a new thread
//            MediaCentre.Playlist.PlayTrackList(tracks, queue);
//        }


//        //public void PlaySimilarTracks(baseEntity entity)
//        //{
//        //    List<String> tracks = new List<string>();

//        //    TrackSimilarDTO dto = new TrackSimilarDTO();
//        //    dto.Artist = entity.ArtistName;
//        //    dto.Track = entity.TrackName;

//        //    TrackSimilarService service = new TrackSimilarService();
//        //    WebServiceProvider webProvider = new LastFMWebProvider();
//        //    service.SetProvider(webProvider);
//        //    service.Fetch(dto);

//        //    if (dto.Status == WebServiceStatus.Error)
//        //    {
//        //        Models.UINotifier.GetInstance().Message = String.Format("error finding tracks similar to {0}", entity.Title);
//        //        return;
//        //    }
//        //    if (dto.Tracks == null || dto.Tracks.Count() == 0)
//        //    {
//        //        Models.UINotifier.GetInstance().Message = String.Format("no tracks are similar to {0}", entity.Title);
//        //        return;
//        //    }

//        //    int maxTracks = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
            
//        //    EntityCollection lib = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track);
//        //    // these cause problems for the locator, the fewer tracks we're dealing with the better
//        //    lib.RemoveAll(item => String.IsNullOrEmpty(item.ArtistName) || String.IsNullOrEmpty(item.TrackName));

//        //    foreach (LfmTrack track in dto.Tracks)
//        //    {
//        //        baseEntity e = LocateTrack(lib, track);
//        //        if (e == null)
//        //        {
//        //            continue;
//        //        }
//        //        tracks.Add(e.Path);
//        //        if (tracks.Count > maxTracks)
//        //        {
//        //            break;
//        //        }
//        //    }

//        //    if (tracks.Count == 0)
//        //    {
//        //        Models.UINotifier.GetInstance().Message = String.Format("no tracks in your library are similar to {0}", entity.Title);
//        //        return;
//        //    }

//        //    // put the focus track in the list
//        //    tracks.Add(entity.Path);
//        //    // dedupe the list
//        //    tracks = tracks.Distinct().ToList();

//        //    // play the tracks
//        //    MediaCentre.Playlist.PlayTrackList(tracks, false);
//        //}

//        //private baseEntity LocateTrack(EntityCollection lib, LfmTrack info)
//        //{
//        //    return lib
//        //        .Where(item => item.ArtistName.Equals(info.artist, StringComparison.CurrentCultureIgnoreCase) ||
//        //            item.TrackName.Equals(info.track, StringComparison.CurrentCultureIgnoreCase))
//        //        .FirstOrDefault();
//        //}

//        #region IBackgroundTaskable Members
//        public string Title
//        {
//            get { return GetType().ToString(); }
//        }

//        public void Execute()
//        {
//            if (_entity.GetType() == typeof(Home)) 
//            { 
//                //TODO: fix
//            }
//            else
//            {
//                List<string> pathList = new List<string>();
//                pathList.Add(_entity.Path);
//            }

//            switch (_action)
//            {
//                //case "cmdsimilar":
//                //    {
//                //        PlaySimilarTracks(_entity);
//                //        break;
//                //    }
//                //case "cmdplayall":
//                //    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
//                //    {
//                //        MediaCentre.Playlist.PlayTrack(_entity, false);
//                //    }
//                //    else
//                //    {
//                //        CreatePlaylist(paths, false, false);
//                //    }
//                //    break;
//                //case "cmdaddtoqueue":
//                //    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
//                //    {
//                //        MediaCentre.Playlist.PlayTrack(_entity, true);
//                //    }
//                //    else
//                //    {
//                //        CreatePlaylist(paths, true, false);
//                //    }
//                //    break;
//                //case "cmdshuffle":
//                //    if (_entity.Kind.Equals(EntityKind.Track) || _entity.Kind.Equals(EntityKind.Playlist))
//                //    {
//                //        MediaCentre.Playlist.PlayTrack(_entity, false);
//                //    }
//                //    else
//                //    {
//                //        CreatePlaylist(paths, false, true);
//                //    }
//                //    break;
//                //case "cmdfavourited":
//                //case "cmdmostplayed":
//                //case "cmdnew":
//                //case "cmdlastfm":
//                //case "cmdrandom":
//                //case "cmdlastfmpopular":
//                //    {
//                //        int size = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
//                //        List<string> tracks = new List<string>();
//                //        switch (_action)
//                //        {
//                //            case "cmdfavourited":
//                //                tracks.AddRange(FindFavorites()); break;
//                //            case "cmdmostplayed":
//                //                tracks.AddRange(FindMostPlayed(size)); break;
//                //            case "cmdnew":
//                //                tracks.AddRange(FindRecentlyAdded(size)); break;
//                //            case "cmdrandom":
//                //                tracks.AddRange(FindRandomPlayed(size, size * 5)); break;
//                //            case "cmdlastfm":
//                //                tracks.AddRange(FindPopularOnLastFM(size)); break;
//                //            case "cmdlastfmpopular":
//                //                tracks.AddRange(FindRandomPopularOnLastFM(size, size * 10)); break;
//                //        }
//                //        MediaCentre.Playlist.PlayTrackList(tracks, false);
//                //        break;
//                //    }
//            }
//        }
//        #endregion
//    }
//}
