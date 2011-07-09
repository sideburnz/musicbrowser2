using System;
using System.IO;
using System.Reflection;
using MusicBrowser.Interfaces;

namespace MusicBrowser.CacheEngine
{
    class CacheEngineFactory
    {
        private static ICacheEngine _cacheEngine;
        private static readonly object Obj = new object();

        public static ICacheEngine GetCacheEngine()
        {
            if (_cacheEngine == null)
            {
                string libraryName = Util.Config.GetInstance().GetSetting("CacheEngine");
                lock (Obj)
                {
                    switch (libraryName.ToLower())
                    {
                        case "none":
                            {
                                _cacheEngine = new DummyCacheEngine();
                                break;
                            }
                        case "filesystem":
                            {
                                _cacheEngine = new FileSystemCacheEngine();
                                break;
                            }
                        default:
                            {
                                _cacheEngine = LoadExternalEngine(libraryName);
                                break;
                            }
                    }
                    if (_cacheEngine == null)
                    {
                        _cacheEngine = new FileSystemCacheEngine();
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
                    Logging.Logger.Error(e);
                }
            }
            return null;
        }
    }
}
