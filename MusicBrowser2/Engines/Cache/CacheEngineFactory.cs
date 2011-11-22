﻿using System;
using System.IO;
using System.Reflection;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Cache
{
    class CacheEngineFactory
    {
        private static ICacheEngine _cacheEngine;
        private static readonly object Obj = new object();

        public static ICacheEngine GetEngine()
        {
            if (_cacheEngine == null)
            {
                bool enable = Util.Config.GetInstance().GetBooleanSetting("Cache.Enable");
                lock (Obj)
                {
                    if (enable)
                    {
                        string engine = Util.Config.GetInstance().GetStringSetting("Cache.Engine");

                        if (engine.ToLower() == "filesystem")
                        {
                            _cacheEngine = new FileSystemCache();
                        }
                        else if (engine.ToLower() == "sqlite")
                        {
                            _cacheEngine = new SQLLiteCache();
                        }
                        else
                        {
                            _cacheEngine = LoadExternalEngine(engine);
                        }
                    }
                    if (!enable)
                    {
                        _cacheEngine = new NoCache();
                    }
                }
            }
            return _cacheEngine;
        }

        public static ICacheEngine LoadExternalEngine(string typeName)
        {
            string libraryPath = Path.Combine(Util.Helper.PlugInFolder, typeName) + ".dll";

            if (File.Exists(libraryPath))
            {
                try
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(libraryPath);
                    return (ICacheEngine)Activator.CreateInstance(pluginAssembly.GetType("MusicBrowser.CacheEngine." + typeName));
                }
                catch(Exception e)
                {
                    Engines.Logging.LoggerEngineFactory.Error(e);
                }
            }
            return null;
        }
    }
}
