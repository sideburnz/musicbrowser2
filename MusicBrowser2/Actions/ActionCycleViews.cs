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

        public override void DoAction(Entity entity)
        {
            string view = Util.Config.GetInstance().GetStringSetting(entity.KindName + ".View");

            switch (view.ToLower())
            {
                case "list": view = "thumb"; break;
                case "thumb": view = "thumbsdown"; break;
                case "thumbsdown": view = "strip"; break;
                case "strip": view = "list"; break;
                default: view = "list"; break;
            }

            Util.Config.GetInstance().SetSetting(entity.KindName + ".View", view);
        }
    }
}
