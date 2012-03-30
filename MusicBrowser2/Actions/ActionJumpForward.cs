using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionJumpBack : baseActionCommand
    {
        private const string LABEL = "Jump Back";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRewind";

        public ActionJumpBack(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionJumpBack()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionJumpBack(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().JumpBack();
        }
    }
}
