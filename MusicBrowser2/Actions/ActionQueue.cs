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

        public ActionQueue(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionQueue(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("queuing {0}", entity.Title);
            TransportEngineFactory.GetEngine().Play(true, entity.Path);
        }
    }
}
