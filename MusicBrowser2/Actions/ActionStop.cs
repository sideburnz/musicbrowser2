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
    public class ActionStop : baseActionCommand
    {
        private const string LABEL = "Stop";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconStop";

        public ActionStop(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionStop()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionStop(entity);
        }

        public override void DoAction(Entity entity)
        {
            TransportEngineFactory.GetEngine().Stop();
        }
    }
}
