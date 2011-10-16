using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    class ActionCloseMenu : baseActionCommand
    {
        public ActionCloseMenu(Entity entity) : base(entity)
        {
            Label = "Close";
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/IconClose";
        }

        public override void DoAction(Entity entity)
        {
            // do nothing
        }
    }
}
