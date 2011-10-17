using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    class ActionOpenEntity : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionOpenEntity(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionOpenEntity()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override void DoAction(Entity entity)
        {
            MusicBrowser.Application.GetReference().Navigate(entity);
        }
    }
}
