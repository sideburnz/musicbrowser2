using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionPause : baseActionCommand
    {
        private const string LABEL = "Pause";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPause";

        public ActionPause(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPause()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPause(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().PlayPause();
        }
    }
}
