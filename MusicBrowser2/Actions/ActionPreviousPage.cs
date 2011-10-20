using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionPreviousPage : baseActionCommand
    {
        private const string LABEL = "Back";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconAction";

        public ActionPreviousPage(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPreviousPage()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override void DoAction(Entity entity)
        {
            Application.GetReference().Session().BackPage();
        }
    }
}
