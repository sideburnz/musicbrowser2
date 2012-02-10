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
    class ActionPlayNewlyAdded : baseActionCommand, IBackgroundTaskable
    {
        private const string LABEL = "Play Newly Added";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayNewlyAdded(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayNewlyAdded()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayNewlyAdded(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "tracks recently added to your library");
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
                .OrderByDescending(item => item.TimeStamp)
                .Take(playlistsize)
                .Select(item => item.Path);
            MusicBrowser.MediaCentre.Playlist.PlayTrackList(items, false);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
