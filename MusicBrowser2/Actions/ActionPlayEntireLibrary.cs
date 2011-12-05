using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    class ActionPlayEntireLibrary : baseActionCommand
    {
        private const string LABEL = "Play All Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayEntireLibrary(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayEntireLibrary()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayEntireLibrary(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "your entire library");
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdplayall", entity), true);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
