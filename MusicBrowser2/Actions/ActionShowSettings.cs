using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowSettings : baseActionCommand
    {
        private const string LABEL = "Settings";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        public ActionShowSettings(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowSettings()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowSettings(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Application.GetReference().NavigateToSettings();
        }
    }
}
