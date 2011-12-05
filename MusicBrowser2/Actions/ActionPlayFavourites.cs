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
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdfavourited", entity), true);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
