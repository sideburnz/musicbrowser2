using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Providers;

// in memory caching, once populated makes browsing faster
namespace MusicBrowser.Engines.Cache
{
    public sealed class InMemoryCache
    {
        private Dictionary<string, baseEntity> _cache = new Dictionary<string, baseEntity>(1000);
        private static readonly object Obj = new object();
        
        #region singleton
        static InMemoryCache _instance;

        public static InMemoryCache GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (Obj)
            {
                if (_instance == null)
                {
                    _instance = new InMemoryCache();
                }
                return _instance;
            }
        }

        #endregion

        // gets the full cache back as an EntityCollection
        public EntityCollection DataSet
        {
            get
            {
                EntityCollection ret = new EntityCollection();
                ret.AddRange(_cache.Values);
                return ret;
            }
        }

        public void Clear()
        {
            lock (Obj)
            {
                _cache = new Dictionary<string, baseEntity>();
            }
        }

        public void Update(baseEntity entity)
        {
            lock (Obj)
            {
                if (_cache.ContainsKey(entity.CacheKey))
                {
                    _cache[entity.CacheKey] = entity;
                }
                else
                {
                    _cache.Add(entity.CacheKey, entity);
                }
            }
        }

        public baseEntity Fetch(string key)
        {
            if (_cache.ContainsKey(key))
            {
                baseEntity e = _cache[key];
                Statistics.Hit("MemCache.Hit");
                return e;
            }
            Statistics.Hit("MemCache.Miss");
            return null;
        }

        public void Remove(string key)
        {
            lock (Obj)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
            }
        }
    }
}
