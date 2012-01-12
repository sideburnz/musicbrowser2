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
            if (entity.FirstPlayed < DateTime.Parse("01-JAN-1000"))
            {
                entity.FirstPlayed = DateTime.Now;
            }
            entity.TimesPlayed++;
            entity.LastPlayed = DateTime.Now;
            entity.UpdateCache();
        }

        public static void SetProgress(this baseEntity entity, int progress)
        {
            if (Util.Helper.InheritsFrom<Item>(entity))
            {
                ((Video)entity).Progress = progress;
                if (progress == 0)
                {
                    entity.LastPlayed = DateTime.Now;
                    entity.TimesPlayed++;
                }
                entity.UpdateCache();
            }
        }
    }
}
