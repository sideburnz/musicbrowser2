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

        public ActionPlayFavourites(Entity entity)
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

        public override void DoAction(Entity entity)
        {
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdfavourited", entity), true);   
        }
    }
}
