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
    public class ActionJumpBack : baseActionCommand
    {
        private const string LABEL = "Jump Back";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRewind";

        public ActionJumpBack(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionJumpBack()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionJumpBack(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().JumpBack();
        }
    }
}
