using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using System.Data;
using ServiceStack.Text;
using System.IO;
using MusicBrowser.Entities.Kinds;

// in memory caching, intended to allow faster searches
//TODO: write scavenger to clean up dead items
//TODO: implement a IsFresh type check for use with the Factory (or scavenger)
//TODO: review to ensure is implemented in a smart manner

namespace MusicBrowser.CacheEngine
{
    public class NearLineCache
    {
        private Dictionary<string, Song> _cache;
        private static readonly object _obj = new object();
        private readonly string _cacheFile = System.IO.Path.Combine(Util.Config.GetInstance().GetSetting("CachePath"), "cache.json");

        #region singleton
        static NearLineCache _instance;

        public static NearLineCache GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_obj)
            {
                if (_instance == null)
                {
                    _instance = new NearLineCache();
                }
                return _instance;
            }
        }
        #endregion

        public void Update(IEntity entity)
        {
            if (!(entity.Kind == EntityKind.Song)) { return; }

            lock (_obj)
            {
                if (_cache.ContainsKey(entity.CacheKey))
                {
                    _cache[entity.CacheKey] = (Song)entity;
                }
                else
                {
                    _cache.Add(entity.CacheKey, (Song)entity);
                }
            }
        }

        public IEnumerable<string> FindFavorites()
        {
            return _cache.Where(item => ((item.Value.Rating == 5) || (item.Value.Favorite))).Select(item => item.Value.Path);
        }

        public IEnumerable<string> FindMostPlayed(int records)
        {
            return _cache.OrderBy(item => item.Value.PlayCount).Reverse().Take(records).Select(item => item.Value.Path);
        }

        public IEnumerable<string> FindRecentlyAdded(int records)
        {
            return _cache.OrderBy(item => item.Value.Added).Reverse().Take(records).Select(item => item.Value.Path);
        }

        public IEntity Fetch(string key)
        {
            if (_cache.ContainsKey(key))
            {
                Statistics.GetInstance().Hit("NLCache.Hit");
                return _cache[key];
            }
            Statistics.GetInstance().Hit("NLCache.Miss");
            return null;
        }

        public void Remove(string key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        }

        public void Save()
        {
            //
            FileStream file = new FileStream(_cacheFile, FileMode.Create);
            JsonSerializer.SerializeToStream<Dictionary<string, Song>>(_cache, file);
            file.Close();

            Logging.Logger.Debug("Saving NL cache to disc: Rows Saved = " + _cache.Count);
        }

        public void Init()
        {
            if (System.IO.File.Exists(_cacheFile))
            {
                try
                {
                    FileStream file = new FileStream(_cacheFile, FileMode.OpenOrCreate);
                    _cache = JsonSerializer.DeserializeFromStream<Dictionary<string, Song>>(file);
                    Logging.Logger.Debug("Loading NL cache from disc: Rows Loaded = " + _cache.Count);
                }
                catch(Exception ex)
                {
                    System.IO.File.Delete(_cacheFile);
                    _cache = new Dictionary<string, Song>(1000);

                    Logging.Logger.Error(ex);
                }
            }
            else
            {
                _cache = new Dictionary<string, Song>(1000);
                Logging.Logger.Debug("NL Cache couldn't be found");
            }
        }
    }
}
