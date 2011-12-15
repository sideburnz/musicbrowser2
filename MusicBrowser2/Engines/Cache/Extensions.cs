using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public static class Extensions
    {
        private static ICacheEngine _engine = CacheEngineFactory.GetEngine();

        public static void UpdateCache(this baseEntity entity)
        {
            _engine.Update(entity);
        }

        public static void MarkPlayed(this baseEntity entity)
        {
            PlayState.MarkPlayed(entity.CacheKey, -1);
        }

        public static void MarkPlayed(this baseEntity entity, int progress)
        {
            PlayState.MarkPlayed(entity.CacheKey, progress);
        }

        public static DateTime LastPlayed(this baseEntity entity)
        {
            return PlayState.LastPlayed(entity.CacheKey);
        }

        public static int PlayProgress(this baseEntity entity)
        {
            return PlayState.Progress(entity.CacheKey);
        }
    }
}
