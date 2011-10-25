using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    class ActionOpen : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionOpen(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionOpen()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOpen(entity);
        }

        public override void DoAction(Entity entity)
        {
            MusicBrowser.Application.GetReference().Navigate(entity);
        }
    }
}
