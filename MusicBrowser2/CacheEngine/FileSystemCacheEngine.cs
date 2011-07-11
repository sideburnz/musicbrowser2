using System;
using System.IO;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;

namespace MusicBrowser.CacheEngine
{
    class FileSystemCacheEngine : ICacheEngine
    {
        private readonly string _cacheLocation = Config.GetInstance().GetSetting("CachePath") + "\\Entities\\";
        private readonly object _obj = new object();

        public FileSystemCacheEngine()
        {
            Helper.BuildCachePath(Config.GetInstance().GetSetting("CachePath"));
        }

        public void Delete(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            if (File.Exists(fileName)) { File.Delete(fileName); }
        }

        public string Read(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);

            if (File.Exists(fileName))
            {
                StreamReader file = new StreamReader(fileName);
                string cachedValue = file.ReadToEnd();
                file.Close();
                return cachedValue;
            }
            return string.Empty;
        }

        public void Update(string key, string value)
        {
            string fileName = CalculateCacheFileFromKey(key);
            lock (_obj)
            {
                StreamWriter file = new StreamWriter(fileName);
                file.Write(value);
                file.Close();
            }
        }

        public bool Exists(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            return File.Exists(fileName);
        }

        public bool IsValid(string key, params DateTime[] comparisons)
        {
            string fileName = CalculateCacheFileFromKey(key);
            DateTime cacheDate = File.GetLastWriteTime(fileName);
            foreach (DateTime d in comparisons)
            {
                if (d > cacheDate) { return false; }
            }
            return true;
        }

        private string CalculateCacheFileFromKey(string key)
        {
            string temp =  _cacheLocation + "\\" + key.Substring(0, 2) + "\\";
            Directory.CreateDirectory(temp);
            return temp + key + ".cache.xml";
        }
    }
}
