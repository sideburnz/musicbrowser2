﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using System.Data;
using ServiceStack.Text;
using System.IO;
using MusicBrowser.Providers.Background;

// in memory caching, intended to allow faster searches
//TODO: implement a IsFresh type check for use with the Factory (or scavenger)
//TODO: review to ensure is implemented in a smart manner

namespace MusicBrowser.CacheEngine
{
    public class NearLineCache : IBackgroundTaskable
    {
        private Dictionary<string, Entity> _cache;
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
        
        public IEnumerable<string> FindFavorites()
        {
            return _cache
                .Where(item => ((item.Value.Rating >= 90) || (item.Value.Favorite)) && (item.Value.Kind == EntityKind.Song))
                .Select(item => item.Value.Path);
        }

        public IEnumerable<string> FindMostPlayed(int records)
        {
            return _cache
                .Where(item => item.Value.Kind == EntityKind.Song)
                .OrderByDescending(item => item.Value.PlayCount)
                .Take(records)
                .Select(item => item.Value.Path);
        }

        public IEnumerable<string> FindRecentlyAdded(int records)
        {
            return _cache
                .Where(item => item.Value.Kind == EntityKind.Song)
                .OrderByDescending(item => item.Value.Added)
                .Take(records)
                .Select(item => item.Value.Path);
        }

        public IEnumerable<string> FindRandomPlayed(int records, int sample)
        {
            return _cache
                .Where(item => item.Value.Kind == EntityKind.Song)
                .OrderByDescending(item => item.Value.PlayCount)
                .Take(sample)
                .OrderBy(item => Guid.NewGuid())
                .Take(records)
                .Select(item => item.Value.Path);
        }

        public Entity Fetch(string key)
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
            lock (_obj)
            {
                if (_cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
            }
        }

        public void Save()
        {
            //
            FileStream file = new FileStream(_cacheFile, FileMode.Create);
            JsonSerializer.SerializeToStream<Dictionary<string, Entity>>(_cache, file);
            file.Close();

            Statistics.GetInstance().Hit("NLCache.Saved." + _cache.Count);
        }

        public void Load()
        {
            if (System.IO.File.Exists(_cacheFile))
            {
                try
                {
                    FileStream file = new FileStream(_cacheFile, FileMode.OpenOrCreate);
                    _cache = JsonSerializer.DeserializeFromStream<Dictionary<string, Entity>>(file);
                    Statistics.GetInstance().Hit("NLCache.Loaded." + _cache.Count);
                }
                catch(Exception ex)
                {
                    System.IO.File.Delete(_cacheFile);
                    _cache = new Dictionary<string, Entity>(1000);

                    Logging.Logger.Error(ex);
                }
            }
            else
            {
                _cache = new Dictionary<string, Entity>(1000);
            }
        }


        #region background task
        public string Title
        {
            get { return "NearLineCache Scavenger"; }
        }

        /// <summary>
        /// This is a scavenger process to help ensure that the NearLine cache doesn't become bloated with
        /// expired data
        /// </summary>
        public void Execute()
        {
            //TODO: find out why this is failing

            //string[] keys = _cache.Keys.ToArray();
            //foreach (string key in keys)
            //{
            //    FileSystemItem item = FileSystemProvider.GetItemDetails(_cache[key].Path);
            //    if (string.IsNullOrEmpty(item.Name)) 
            //    { 
            //        Remove(key); 
            //        Statistics.GetInstance().Hit("NLCache.Scavenged.Gone"); 
            //        continue; 
            //    }
            //    if (item.LastUpdated > _cache[key].CacheDate) 
            //    { 
            //        Remove(key); 
            //        Statistics.GetInstance().Hit("NLCache.Scavenged.Expired"); 
            //        continue; 
            //    }
            //}
        }
        #endregion
    }
}
