using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowActions : baseActionCommand
    {
        private const string LABEL = "Show Menu";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionShowActions(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionShowActions()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public string Source { get; set; }

        public override void DoAction(Entity entity)
        {
            Providers.Statistics.Hit("ActionTrigger." + Source);
            Models.ActionsModel.GetInstance.Visible = true;
        }
    }
}
