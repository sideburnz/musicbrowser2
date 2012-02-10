using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Actions;

namespace MusicBrowser.Engines.PlugIns.Actions
{
    public class ActionPlayRandomPopularLastFM : baseActionCommand
    {
        private const string LABEL = "Play Random Popular";
        private const string ICON_PATH = "resx://LastFMProvider/LastFMProvider.Resources/IconLastFM"; 

        public ActionPlayRandomPopularLastFM(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
            Entity = entity;
        }

        public ActionPlayRandomPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayRandomPopularLastFM(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            //Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "random tracks with the high playcounts on Last.fm");
            //CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfmpopular", entity), true);
            //MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
