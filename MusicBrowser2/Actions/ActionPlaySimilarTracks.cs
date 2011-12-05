using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{

    /// <summary>
    /// This is a first cut as a iTunes-Genius style playlist creator based on the Last.fm Track.getSimilar API
    /// </summary>
    public class ActionPlaySimilarTracks : baseActionCommand
    {
        private const string LABEL = "Play Similar Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconLastFM";

        public ActionPlaySimilarTracks(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public ActionPlaySimilarTracks()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlaySimilarTracks(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdsimilar", entity), true);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }

    }
}
