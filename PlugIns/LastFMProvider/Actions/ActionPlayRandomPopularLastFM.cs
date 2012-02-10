using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Actions;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.PlugIns.Actions
{
    public class ActionPlayRandomPopularLastFM : baseActionCommand, IBackgroundTaskable
    {
        private const string LABEL = "Play Random Popular (Last.fm)";
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
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "random tracks with the high playcounts on Last.fm");
            CommonTaskQueue.Enqueue(this, true);
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
                .Take(playlistsize * 5)
                .ToList()
                .ShuffleList()
                .Take(playlistsize)
                .Select(item => item.Path);
            MusicBrowser.MediaCentre.Playlist.PlayTrackList(items, false);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
