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
using ServiceStack.Text;

// in memory caching, intended to allow faster searches
namespace MusicBrowser.Engines.Cache
{
    public sealed class InMemoryCache //: IBackgroundTaskable
    {
        private Dictionary<string, Entity> _cache;
        private static readonly object _obj = new object();
        private readonly string _cacheFile = System.IO.Path.Combine(Util.Config.GetInstance().GetStringSetting("Cache.Path"), "cache.xml");

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

        private InMemoryCache()
        {
            Load();
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

        public void Update(Entity entity)
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

        public Entity Fetch(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                Statistics.Hit("MemCache.Miss");
                return null;
            }
            Entity e = _cache[key];
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

        private void Load()
        {
            if (System.IO.File.Exists(_cacheFile))
            {
                try
                {
                    FileStream file = new FileStream(_cacheFile, FileMode.OpenOrCreate);
                    _cache = XmlSerializer.DeserializeFromStream<Dictionary<String, Entity>>(file);
                    //_cache = JsonSerializer.DeserializeFromStream<Dictionary<string, Entity>>(file);
                    Statistics.Hit("InMemoryCache.Loaded", _cache.Count);
                }
                catch (Exception ex)
                {
                    _cache = new Dictionary<string, Entity>(1000);
                    Engines.Logging.LoggerEngineFactory.Error(ex);
                    try
                    {
                        System.IO.File.Delete(_cacheFile);
                    }
                    catch { }
                }
            }
            else
            {
                _cache = new Dictionary<string, Entity>(1000);
            }
        }

        public void Save()
        {
            try
            {
                FileStream file = new FileStream(_cacheFile, FileMode.Create);
                XmlSerializer.SerializeToStream(_cache, file);
                    //.SerializeToStream<Dictionary<string, Entity>>(_cache, file);
                file.Close();
                Statistics.Hit("InMemoryCache.Saved", _cache.Count);
            }
            catch
            {
                File.Delete(_cacheFile);
            }
        }
    }
}
