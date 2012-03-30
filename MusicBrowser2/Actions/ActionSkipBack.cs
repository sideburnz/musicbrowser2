using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionSkipBack : baseActionCommand
    {
        private const string LABEL = "Skip Forward";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconSkipBack";

        public ActionSkipBack(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSkipBack()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSkipBack(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().Previous();
        }
    }
}
