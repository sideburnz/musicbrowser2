using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;

namespace MusicBrowser.Engines.Cache
{
    /// <summary>
    /// This is an empty implementation of the CacheEngine, it effectively turns the Cache off
    /// </summary>
    class NoCache : ICacheEngine
    {
        public void Delete(string key)
        { }

        public byte[] Fetch(string key)
        {
            return null;
        }

        public void Update(string key, byte[] entity) 
        { }

        public bool Exists(string key) 
        {
            return false;
        }

        public void Scavenge() { }

        public void Clear() { }
    }
}
