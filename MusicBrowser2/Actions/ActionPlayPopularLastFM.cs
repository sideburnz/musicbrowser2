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
        private const string LABEL = "Play popular on Last.fm";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconLastFM";

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
            return new ActionPlayPopularLastFM(entity);
        }

        public override void DoAction(Entity entity)
        {
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfm", entity), true);   
        }
    }
}
