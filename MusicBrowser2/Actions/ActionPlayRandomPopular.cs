using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    class ActionPlayRandomPopular : baseActionCommand
    {
        private const string LABEL = "Play Random Popular";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayRandomPopular(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayRandomPopular()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override void DoAction(Entity entity)
        {
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdrandom", entity), true);   
        }
    }
}
