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
    public class ActionPause : baseActionCommand
    {
        private const string LABEL = "Pause";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPause";

        public ActionPause(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPause()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPause(entity);
        }

        public override void DoAction(Entity entity)
        {
            TransportEngineFactory.GetEngine().PlayPause();
        }
    }
}
