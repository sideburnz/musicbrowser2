using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnStar : baseActionCommand
    {
        private const string LABEL = "Star";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnStar(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnStar()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnStar(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetStarAction(entity);
            action.Invoke();
        }
    }
}
