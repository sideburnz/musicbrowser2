using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    class ActionCloseMenu : baseActionCommand
    {
        private const string LABEL = "Cancel";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconClose";

        public ActionCloseMenu(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCloseMenu()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionCloseMenu(entity);
        }

        public override void DoAction(Entity entity)
        {
            // do nothing
        }
    }
}
