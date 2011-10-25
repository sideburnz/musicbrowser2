using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnPlay : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnPlay(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionOnPlay()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOnPlay(entity);
        }

        public override void DoAction(Entity entity)
        {
            baseActionCommand action = Helper.GetPlayAction(entity);
            action.Invoke();
        }
    }
}
