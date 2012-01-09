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
    public class ActionJumpForward : baseActionCommand
    {
        private const string LABEL = "Jump Forward";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconFastForward";

        public ActionJumpForward(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionJumpForward()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionJumpForward(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().JumpForward();
        }
    }
}
