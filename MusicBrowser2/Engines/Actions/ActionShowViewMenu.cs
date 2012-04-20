using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Engines.Actions
{
    public class ActionShowViewMenu : baseActionCommand
    {
        private const string LABEL = "View Menu";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconView";

        public ActionShowViewMenu(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionShowViewMenu()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowViewMenu(entity);
        }

        public string Source { get; set; }

        public override void DoAction(baseEntity entity)
        {
            ViewMenuModel.GetInstance.Visible = true;
        }
    }
}
