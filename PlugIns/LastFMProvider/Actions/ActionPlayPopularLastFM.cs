using System;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.MediaCentre;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace MusicBrowser.Engines.PlugIns.Actions
{
    public class ActionPlayPopularLastFM : baseActionCommand, IBackgroundTaskable
    {
        private const string LABEL = "Play Popular";
        private const string ICON_PATH = "resx://LastFMProvider/LastFMProvider.Resources/IconLastFM";       

        public ActionPlayPopularLastFM(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
            Entity = entity;
        }

        public ActionPlayPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public string Title
        {
            get { return Label; }
        }

        public void Execute()
        {
            int playlistsize = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");

            IEnumerable<string> items = Engines.Cache.InMemoryCache.GetInstance().DataSet
                .Where(item => item.Kind == "Track")
                .OrderByDescending(item => ((Track)item).LastFMPlayCount)
                .Take(playlistsize)
                .Select(item => item.Path);

            Playlist.PlayTrackList(items, false);
            Playlist.AutoShowNowPlaying();
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayPopularLastFM(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "tracks with the highest playcounts on Last.fm");
            CommonTaskQueue.Enqueue(this, true);
        }
    }
}
