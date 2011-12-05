using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    public class ActionPlayMostPopular : baseActionCommand
    {
        private const string LABEL = "Play Most Popular";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayMostPopular(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }

        public ActionPlayMostPopular()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayMostPopular(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "your most played tracks");
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdmostplayed", entity), true);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
