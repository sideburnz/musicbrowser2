using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionNoOperation : baseActionCommand
    {
        private const string LABEL = "Unknown";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconUnknown";

        public ActionNoOperation(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionNoOperation(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            // do absolutely nothing
        }
    }
}
