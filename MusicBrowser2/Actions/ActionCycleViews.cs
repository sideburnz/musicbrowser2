using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{
    public class ActionCycleViews : baseActionCommand
    {
        private const string LABEL = "Change View";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconView";

        public ActionCycleViews(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionCycleViews(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            string view = entity.View;
            switch (view.ToLower())
            {
                case "list": view = "Thumb"; break;
                case "thumb": view = "Strip"; break;
                case "strip": view = "List"; break;
                default: view = "List"; break;
            }
            entity.View = view;
            entity.UpdateCache();
        }
    }
}
