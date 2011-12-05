using MusicBrowser.Entities;
using System.IO;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System;

namespace MusicBrowser.Actions
{
    class ActionOpen : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionOpen(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOpen(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            if (Directory.Exists(entity.Path) || InheritsFrom(entity, "MusicBrowser.Entity.Collection"))
            {
                MusicBrowser.Application.GetReference().Navigate(entity);
                //TODO: assess if this is needed
                //CommonTaskQueue.Enqueue(new BackgroundCacheProvider(Entity.Path), true);
            }
        }
    }
}
