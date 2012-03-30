using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnRecord : baseActionCommand
    {
        private const string LABEL = "On Record";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnRecord(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnRecord()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOnRecord(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            baseActionCommand action = Factory.GetRecordAction(entity);
            action.Invoke();
        }
    }
}
