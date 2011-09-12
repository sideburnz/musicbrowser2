using System;
using System.IO;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;
using MusicBrowser.Providers;

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
            lock (_obj)
            {
                if (File.Exists(fileName)) { File.Delete(fileName); }
            }
        }

        public string Read(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            lock (_obj)
            {
                if (File.Exists(fileName))
                {
                    StreamReader file = new StreamReader(fileName);
                    string cachedValue = file.ReadToEnd();
                    file.Close();
                    return cachedValue;
                }
            }
            return string.Empty;
        }

        public void Update(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                string fileName = CalculateCacheFileFromKey(key);
                lock (_obj)
                {
                    StreamWriter file = new StreamWriter(fileName);
                    file.Write(value);
                    file.Close();
                }
            }
            else
            {
                Delete(key);
            }
        }

        public bool Exists(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            return File.Exists(fileName);
        }

        public DateTime GetAge(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            FileSystemItem item = FileSystemProvider.GetItemDetails(fileName);
            return item.LastUpdated;
        }

        private string CalculateCacheFileFromKey(string key)
        {
            string temp =  string.Concat(_cacheLocation, "\\", key.Substring(0, 2), "\\");
            Directory.CreateDirectory(temp);
            return temp + key + ".cache.xml";
        }
    }
}
