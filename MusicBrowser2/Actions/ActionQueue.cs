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
    class ActionQueue : baseActionCommand
    {
        private const string LABEL = "Queue";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconQueue";

        public ActionQueue(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionQueue()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionQueue(entity);
        }

        public override void DoAction(Entity entity)
        {
            TransportEngineFactory.GetEngine().Play(true, entity.Path);
        }
    }
}
