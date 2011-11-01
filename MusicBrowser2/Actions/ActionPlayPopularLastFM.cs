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
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
            Entity = entity;
        }

        public ActionPlayPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlayPopularLastFM(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "tracks with the highest playcounts on Last.fm");
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfm", entity), true);   
        }
    }
}
