using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnMinus : baseActionCommand
    {
        private const string LABEL = "Minus";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnMinus(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnMinus()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnMinus(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetMinusAction(entity);
            action.Invoke();
        }
    }
}
