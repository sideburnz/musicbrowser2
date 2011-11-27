using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnEnter : baseActionCommand
    {
        private const string LABEL = "On Enter";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnEnter(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnEnter()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnEnter(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetEnterAction(entity);
            action.Invoke();
        }
    }
}
