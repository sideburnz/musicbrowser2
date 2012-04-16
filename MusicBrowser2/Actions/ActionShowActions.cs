using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    public class ActionShowActions : baseActionCommand
    {
        private const string LABEL = "Actions";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionShowActions(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowActions(entity);
        }

        public string Source { get; set; }

        public override void DoAction(baseEntity entity)
        {
            Providers.Telemetry.Hit("ActionTrigger." + Source);
            ActionsModel.GetInstance.Context = entity;
            ActionsModel.GetInstance.Visible = true;
        }
    }
}
