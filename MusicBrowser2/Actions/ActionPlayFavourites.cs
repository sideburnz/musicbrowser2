using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    class ActionPlayFavourites : baseActionCommand
    {
        private const string LABEL = "Play Favourites";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconFavorite";

        public ActionPlayFavourites(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayFavourites()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayFavourites(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "tracks with 5 stars or marked as favourite");

            int playlistsize = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
            IEnumerable<string> items = Engines.Cache.InMemoryCache.GetInstance().DataSet
                .Where(item => ((item.Kind == "Track") && (item.Rating >= 90) || (item.Loved)))
                .Take(playlistsize)
                .Select(item => item.Path);
            MusicBrowser.MediaCentre.Playlist.PlayTrackList(items, false);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
