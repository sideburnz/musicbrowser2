using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    public class ActionCloseMenu : baseActionCommand
    {
        private const string LABEL = "Cancel";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconClose";

        public ActionCloseMenu(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionCloseMenu(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            // do nothing
        }
    }
}
