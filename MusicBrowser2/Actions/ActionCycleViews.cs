using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionCycleViews : baseActionCommand
    {
        private const string LABEL = "Change View";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconView";

        public ActionCycleViews(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCycleViews()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionCycleViews(entity);
        }

        public override void DoAction(Entity entity)
        {
            string view = Util.Config.GetInstance().GetStringSetting("Entity." + entity.KindName + ".View");

            switch (view.ToLower())
            {
                case "list": view = "Thumb"; break;
                case "thumb": view = "Strip"; break;
                case "strip": view = "List"; break;
                default: view = "List"; break;
            }

            Util.Config.GetInstance().SetSetting("Entity." + entity.KindName + ".View", view);
        }
    }
}
