using System;
using System.IO;
using System.Reflection;

namespace MusicBrowser.Engines.Cache
{
    class CacheEngineFactory
    {
        private static ICacheEngine _cacheEngine;
        private static readonly object Obj = new object();

        public static ICacheEngine GetEngine()
        {
            if (_cacheEngine == null)
            {
                bool enable = Util.Config.GetInstance().GetBooleanSetting("Cache.Enable");
                lock (Obj)
                {
                    if (enable)
                    {
                        _cacheEngine = new SQLiteCache();
                    }
                    else
                    {
                        _cacheEngine = new NoCache();
                    }
                }
            }
            return _cacheEngine;
        }
    }
}
