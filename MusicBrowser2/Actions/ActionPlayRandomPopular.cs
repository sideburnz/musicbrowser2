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
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }

        public ActionPlayRandomPopular()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlayRandomPopular(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "a random selection of tracks from your library");
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdrandom", entity), true);   
        }
    }
}
