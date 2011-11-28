using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;

namespace MusicBrowser.Actions
{
    public class ActionPlayMusic : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayMusic(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayMusic()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlayMusic(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", entity.Title);
            TransportEngineFactory.GetEngine().Play(false, entity.Path);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
