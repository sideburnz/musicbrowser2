using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    class ActionPlayNewlyAdded : baseActionCommand
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
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdnew", entity), true);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
