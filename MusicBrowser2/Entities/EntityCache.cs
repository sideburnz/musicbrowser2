using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MusicBrowser.Entities.Interfaces;

namespace MusicBrowser.Entities
{
    class EntityCache : IEntityCache
    {
        #region private variables
        private readonly Dictionary<string, IEntity> _memoryCache;
        private readonly string _cacheLocation;
        private readonly bool _cacheDisabled;
        private int _cacheHits;
        private int _cacheMisses;
        private readonly object obj = new object();
        #endregion

        #region constructors
        public EntityCache()
        {
            _cacheHits = 0;
            _cacheMisses = 0;
            _cacheLocation = Util.Helper.AppCachePath + "\\Entities\\";
            _cacheDisabled = !Util.Config.getInstance().getBooleanSetting("EnableCache");
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
            if (_memoryCache.ContainsKey(key))
            {
                _cacheHits++;
                return _memoryCache[key];
            }
            _cacheMisses++;
            loadCacheItemToMemory(key);
            _memoryCache[key].Dirty = false;
            return _memoryCache[key];
        }

        public void Update(string key, IEntity entity)
        {
            if (_cacheDisabled) { return; }
            if (Exists(key) && !entity.Dirty) { return; }

            this.Delete(key);
            _memoryCache.Add(key, entity);
            string fileName = _cacheLocation + key + ".cache.xml";
            // there have been collisions, this fix comes with a performance penalty
            lock (obj)
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

            if (_memoryCache.ContainsKey(key)) { return true; }
            if (File.Exists(_cacheLocation + key + ".cache.xml")) { return true; }
            return false;
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

        public int Hits
        {
            get { return _cacheHits; }
        }

        public int Misses
        {
            get { return _cacheMisses; }
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

        private void loadCacheItemToMemory(string key)
        {
            string fileName = _cacheLocation + key + ".cache.xml";
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            string cacheContent = file.ReadToEnd();
            file.Close();
            _memoryCache.Add(key, EntityPersistance.Deserialize(cacheContent));
        }
    }
}
