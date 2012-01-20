using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{
    public class ActionCycleOrientation : baseActionCommand
    {
        private const string LABEL = "Change Orientation";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconView";

        public ActionCycleOrientation(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCycleOrientation()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionCycleOrientation(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            string setting = "Views.IsHorizontal";

            Util.Config config = Util.Config.GetInstance();
            config.SetSetting(setting, (!config.GetBooleanSetting(setting)).ToString());
        }
    }
}
