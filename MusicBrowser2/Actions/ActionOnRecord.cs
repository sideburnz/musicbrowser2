using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnRecord : baseActionCommand
    {
        private const string LABEL = "Record";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnRecord(Entity entity)
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

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnRecord(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetRecordAction(entity);
            action.Invoke();
        }
    }
}
