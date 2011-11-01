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
    public class ActionSkipBack : baseActionCommand
    {
        private const string LABEL = "Skip Forward";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconSkipBack";

        public ActionSkipBack(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSkipBack()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionSkipBack(entity);
        }

        public override void DoAction(Entity entity)
        {
            TransportEngineFactory.GetEngine().Previous();
        }
    }
}
