using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using System.Data;
using System.IO;
using MusicBrowser.Providers.Background;

// in memory caching, intended to allow faster searches
namespace MusicBrowser.Engines.Cache
{
    public sealed class InMemoryCache
    {
        private Dictionary<string, baseEntity> _cache = new Dictionary<string, baseEntity>(1000);
        private static readonly object _obj = new object();
        
        #region singleton
        static InMemoryCache _instance;

        public static InMemoryCache GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_obj)
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
            lock (_obj)
            {
                _cache = new Dictionary<string, baseEntity>();
            }
        }

        public void Update(baseEntity entity)
        {
            lock (_obj)
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
            if (!_cache.ContainsKey(key))
            {
                Statistics.Hit("MemCache.Miss");
                return null;
            }
            baseEntity e = _cache[key];
            Statistics.Hit("MemCache.Hit");
            return e;
        }

        public void Remove(string key)
        {
            lock (_obj)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
            }
        }
    }
}
