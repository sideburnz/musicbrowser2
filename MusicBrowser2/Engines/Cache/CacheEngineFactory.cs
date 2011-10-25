using System;
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
                string libraryName = Util.Config.GetInstance().GetStringSetting("CacheEngine");
                lock (Obj)
                {
                    switch (libraryName.ToLower())
                    {
                        case "none":
                            {
                                _cacheEngine = new NoCache();
                                break;
                            }
                        case "filesystem":
                            {
                                _cacheEngine = new FileSystemCache();
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
