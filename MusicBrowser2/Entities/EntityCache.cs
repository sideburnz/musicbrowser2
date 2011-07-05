using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    public class EntityCache
    {
        #region private variables
        private readonly Dictionary<string, IEntity> _memoryCache;
        private readonly string _cacheLocation;
        private readonly bool _cacheDisabled;
        private readonly object _obj = new object();
        #endregion

        #region constructors
        public EntityCache()
        {
            _cacheLocation = Config.GetInstance().GetSetting("CachePath") + "\\Entities\\";
            Helper.BuildCachePath(Config.GetInstance().GetSetting("CachePath"));
            _cacheDisabled = !Config.GetInstance().GetBooleanSetting("EnableCache");
            _memoryCache = new Dictionary<string, IEntity>();
        }
        #endregion

        #region IEntityCache Members

        public void Delete(string key)
        {
            string fileName = _cacheLocation + key + ".cache.xml";
            if (File.Exists(fileName)) { File.Delete(fileName); }
            if (_memoryCache.ContainsKey(key)) { _memoryCache.Remove(key); }
        }

        public IEntity Read(string key)
        {
            Providers.Statistics stats = Providers.Statistics.GetInstance();
            if (_memoryCache.ContainsKey(key))
            {
                stats.Hit("cache.memory.hits");
                return _memoryCache[key];
            }
            stats.Hit("cache.memory.misses");
            if (File.Exists(_cacheLocation + key + ".cache.xml"))
            {
                if (LoadCacheItemToMemory(key))
                {
                    stats.Hit("cache.disk.hits");
                    _memoryCache[key].Dirty = false;
                    return _memoryCache[key];
                }
            }
            stats.Hit("cache.disk.misses");
            return null;
        }

        public void Update(string key, IEntity entity)
        {
            if (_cacheDisabled) { return; }
            if (!entity.Dirty) { return; }

            _memoryCache[key] = entity;
            string fileName = _cacheLocation + key + ".cache.xml";
            // there have been collisions, this fix comes with a performance penalty
            lock (_obj)
            {
                StreamWriter file = new StreamWriter(fileName);
                file.Write(EntityPersistance.Serialize(entity));
                file.Close();
            }
            entity.Dirty = false;
        }

        public bool Exists(string key)
        {
            if (_cacheDisabled) { return false; }
            return _memoryCache.ContainsKey(key) || (File.Exists(_cacheLocation + key + ".cache.xml"));
        }

        public bool IsValid(string key, params DateTime[] comparisons)
        {
            if (_cacheDisabled) { return false; }

            string fileName = _cacheLocation + key + ".cache.xml";
            DateTime cacheDate = File.GetLastWriteTime(fileName);
            foreach (DateTime d in comparisons)
            {
                if (d > cacheDate) { return false; }
            }
            return true;
        }

        public int Size
        {
            get 
            {
                if (_cacheDisabled) { return -1; }
                return new DirectoryInfo(_cacheLocation).GetFiles("*.cache.xml").Length;
            }
        }

        #endregion

        private bool LoadCacheItemToMemory(string key)
        {
            string fileName = _cacheLocation + key + ".cache.xml";
            StreamReader file = new StreamReader(fileName);
            string cacheContent = file.ReadToEnd();
            file.Close();

            IEntity cache = EntityPersistance.Deserialize(cacheContent);

            // there's been a problem with the cache, remove it
            if (cache.Kind.Equals(EntityKind.Unknown))
            {
                try { File.Delete(fileName); return false;  }
                catch { }
            }

            _memoryCache.Add(key, cache);
            return true;
        }
    }
}
