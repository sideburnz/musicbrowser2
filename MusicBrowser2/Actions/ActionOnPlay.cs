using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionOnPlay : baseActionCommand
    {
        private const string LABEL = "On Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionOnPlay(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOnPlay(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            baseActionCommand action = Factory.GetPlayAction(entity);
            action.Invoke();
        }
    }
}
