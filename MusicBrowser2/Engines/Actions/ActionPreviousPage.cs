using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionPreviousPage : baseActionCommand
    {
        private const string LABEL = "Close Page";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconClose";

        public ActionPreviousPage(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPreviousPage()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPreviousPage(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Application.GetReference().Session().BackPage();
        }
    }
}
