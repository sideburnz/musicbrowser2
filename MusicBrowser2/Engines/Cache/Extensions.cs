using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public static class Extensions
    {
        private static readonly ICacheEngine Engine = CacheEngineFactory.GetEngine();
        private static readonly InMemoryCache Memory = InMemoryCache.GetInstance();

        public static void UpdateCache(this baseEntity entity)
        {
            Memory.Update(entity);
            Engine.Update(entity);
        }

        public static void MarkPlayed(this baseEntity entity)
        {
            entity.PlayState.Play();
        }

        public static void SetProgress(this Video entity, int progress)
        {
            Logging.LoggerEngineFactory.Debug("Cache.Extensions", String.Format("Video {0} progress recorded as {1}", entity.Title, progress));
            entity.PlayState.Progress = progress;
        }
    }
}

