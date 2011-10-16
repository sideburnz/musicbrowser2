using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    class ActionOpenEntity : baseActionCommand
    {
        public ActionOpenEntity(Entity entity) : base(entity)
        {
            Label = "Open";
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";
        }

        public override void DoAction(Entity entity)
        {
            MusicBrowser.Application.GetReference().Navigate(entity);
        }
    }
}
