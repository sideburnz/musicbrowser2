using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    class ActionQueue : baseActionCommand
    {
        private const string LABEL = "Queue";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconQueue";

        public ActionQueue(baseEntity entity)
        {
            Label = LABEL + " " + entity.Kind;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = !entity.InheritsFrom<Video>(); // videos can be queued
        }

        public ActionQueue()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionQueue(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            entity.Play(true, false);
        }
    }
}
