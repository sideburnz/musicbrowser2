using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;

namespace MusicBrowser.CacheEngine
{
    /// <summary>
    /// This is an empty implementation of the CacheEngine, it effectively turns the Cache off
    /// </summary>
    class DummyCacheEngine : ICacheEngine
    {
        public void Delete(string key)
        { }

        public string Read(string key)
        {
            return String.Empty;
        }

        public void Update(string key, string entity) 
        { }

        public bool Exists(string key) 
        {
            return false;
        }

        public bool IsValid(string key, params DateTime[] comparisons)
        {
            return false;
        }
    }
}
