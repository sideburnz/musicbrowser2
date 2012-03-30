using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowViews : baseActionCommand
    {
        private const string LABEL = "Preconfigured Views";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconOpen";

        public ActionShowViews(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionShowViews()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowViews(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.ViewsModel.GetInstance.Context = entity;
            Models.ViewsModel.GetInstance.Visible = true;
        }
    }
}
