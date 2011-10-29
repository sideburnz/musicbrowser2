using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    public class ActionPlayPopularLastFM : baseActionCommand
    {
        private const string LABEL = "XXXX";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayPopularLastFM(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlayRandomPopular(entity);
        }

        public override void DoAction(Entity entity)
        {
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfm", entity), true);   
        }
    }
}
