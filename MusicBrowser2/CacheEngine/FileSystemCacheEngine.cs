﻿using System;
using System.IO;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;
using MusicBrowser.Providers;

namespace MusicBrowser.CacheEngine
{
    class FileSystemCacheEngine : ICacheEngine
    {
        private readonly string _cacheLocation = Config.GetInstance().GetStringSetting("CachePath") + "\\Entities\\";
        private readonly object _obj = new object();

        public FileSystemCacheEngine()
        {
            Helper.BuildCachePath(Config.GetInstance().GetStringSetting("CachePath"));
        }

        public void Delete(string key)
        {
            string fileName = CalculateCacheFileFromKey(key);
            lock (_obj)
            {
                if (File.Exists(fileName)) { File.Delete(fileName); }
            }
        }

        public string FetchIfFresh(string key, DateTime comparer)
        {
            string fileName = CalculateCacheFileFromKey(key);

            lock (_obj)
            {
                if (File.Exists(fileName))
                {
                    if (File.GetLastWriteTime(fileName) < comparer)
                    {
                        return string.Empty;
                    }
                    StreamReader file = new StreamReader(fileName);
                    string cachedValue = file.ReadToEnd();
                    file.Close();

                    Statistics.GetInstance().Hit("cache.hit");

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

        private string CalculateCacheFileFromKey(string key)
        {
            string temp =  string.Concat(_cacheLocation, "\\", key.Substring(0, 2), "\\");
            Directory.CreateDirectory(temp);
            return temp + key + ".cache.json";
        }


        public void Scavenge()
        {
            throw new NotImplementedException();
        }
    }
}
