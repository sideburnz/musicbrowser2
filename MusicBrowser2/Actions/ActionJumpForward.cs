using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionJumpForward : baseActionCommand
    {
        private const string LABEL = "Jump Forward";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconFastForward";

        public ActionJumpForward(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionJumpForward()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionJumpForward(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            TransportEngineFactory.GetEngine().JumpForward();
        }
    }
}
