using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnPlus : baseActionCommand
    {
        private const string LABEL = "Plus";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnPlus(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnPlus()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnPlus(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetPlusAction(entity);
            action.Invoke();
        }
    }
}
