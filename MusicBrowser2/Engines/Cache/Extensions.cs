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
    }
}
