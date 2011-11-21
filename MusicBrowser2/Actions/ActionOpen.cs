using MusicBrowser.Entities;
using System.IO;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    class ActionOpen : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionOpen(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionOpen()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionOpen(entity);
        }

        public override void DoAction(Entity entity)
        {
            if (Directory.Exists(entity.Path))
            {
                MusicBrowser.Application.GetReference().Navigate(entity);
                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(Entity.Path), true);
            }
        }
    }
}
