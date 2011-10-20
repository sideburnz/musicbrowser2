using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionDefaultAction : baseActionCommand
    {
        private const string LABEL = "Default Action";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionDefaultAction(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionDefaultAction()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public string Source { get; set; }

        public override void DoAction(Entity entity)
        {
            Providers.Statistics.Hit("ActionTrigger." + Source);
            baseActionCommand Action = (baseActionCommand)entity.Actions[0];
            Action.Invoke();
        }
    }
}
