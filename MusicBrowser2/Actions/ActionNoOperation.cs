using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionNoOperation : baseActionCommand
    {
        private const string LABEL = "No Operation";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconClose";

        public ActionNoOperation(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionNoOperation()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionNoOperation(entity);
        }

        public override void DoAction(Entity entity)
        {
            // do absolutely nothing
        }
    }
}
