using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{
    public class ActionIncreaseThumbSize : baseActionCommand
    {
        private const string LABEL = "Increase Thumb";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlus";

        public ActionIncreaseThumbSize(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionIncreaseThumbSize()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionIncreaseThumbSize(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            if (entity.ThumbSize < 300)
            {
                entity.ThumbSize += 25;
                entity.UpdateCache();
            }
        }
    }
}
