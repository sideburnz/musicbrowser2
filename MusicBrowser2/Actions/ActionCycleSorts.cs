using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionCycleSorts : baseActionCommand
    {
        private const string LABEL = "Change Sort Order";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconSort";

        public ActionCycleSorts(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCycleSorts()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionCycleSorts(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            string order = Util.Config.GetInstance().GetStringSetting("Entity." + entity.Kind + ".SortOrder");

            switch (order.ToLower())
            {
                case "[title]": order = "[added]"; break;
                case "[added]": order = "[title]"; break;
                default: order = "[title]"; break;
            }

            Util.Config.GetInstance().SetSetting("Entity." + entity.Kind + ".SortOrder", order);
        }
    }
}
