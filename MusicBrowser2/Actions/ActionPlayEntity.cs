using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Actions
{
    class ActionPlayEntity : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayEntity(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayEntity()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override void DoAction(Entity entity)
        {
            TransportEngineFactory.GetEngine().Play(false, entity.Path);
        }
    }
}
