using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{
    public class ActionDecreaseThumbSize : baseActionCommand
    {
        private const string LABEL = "Decrease Thumb";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconMinus";

        public ActionDecreaseThumbSize(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionDecreaseThumbSize()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionDecreaseThumbSize(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            if (entity.ThumbSize > 100)
            {
                entity.ThumbSize -= 25;
                entity.UpdateCache();
            }
        }
    }
}
