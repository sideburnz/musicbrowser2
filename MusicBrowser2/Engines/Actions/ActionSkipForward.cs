using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionSkipForward : baseActionCommand
    {
        private const string LABEL = "Skip Forward";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconSkipForward";

        public ActionSkipForward(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSkipForward()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSkipForward(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().Next();
        }
    }
}
