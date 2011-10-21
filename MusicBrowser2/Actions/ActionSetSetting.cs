using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionSetSetting : baseActionCommand
    {
        private const string LABEL = "Set Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        public ActionSetSetting(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSetSetting()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public override void DoAction(Entity entity)
        {
            Util.Config.GetInstance().SetSetting(Key, Value);
        }
    }
}
