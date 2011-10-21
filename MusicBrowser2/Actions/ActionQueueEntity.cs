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
    class ActionQueueEntity : baseActionCommand
    {
        private const string LABEL = "Queue";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconQueue";

        public ActionQueueEntity(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionQueueEntity()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override void DoAction(Entity entity)
        {
            Transport.GetTransport().Play(true, entity.Path);
        }
    }
}
