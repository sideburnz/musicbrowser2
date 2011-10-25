using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowSettings : baseActionCommand
    {
        private const string LABEL = "Settings";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        public ActionShowSettings(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowSettings()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionShowSettings(entity);
        }

        public override void DoAction(Entity entity)
        {
            Application.GetReference().NavigateToSettings();
        }
    }
}
