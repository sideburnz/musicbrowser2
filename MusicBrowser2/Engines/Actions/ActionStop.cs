using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionStop : baseActionCommand
    {
        private const string LABEL = "Stop";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconStop";

        public ActionStop(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionStop()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionStop(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().Stop();
        }
    }
}
