using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MusicBrowser.Interfaces;

namespace MusicBrowser.CacheEngine
{
    class CacheEngineFactory
    {
        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private static ICacheEngine _cacheEngine;
        private static readonly object Obj = new object();

        public static ICacheEngine GetCacheEngine()
        {
            if (_cacheEngine == null)
            {
                string libraryName = Util.Config.GetInstance().GetSetting("CacheEngine").ToLower();
                lock (Obj)
                {
                    switch (libraryName)
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

        public static ICacheEngine LoadExternalEngine(string path)
        {
            string libraryPath = Path.Combine(Util.Helper.PlugInFolder, path);
            if (File.Exists(libraryPath))
            {
                Assembly pluginAssembly = Assembly.LoadFrom(libraryPath);
                return (ICacheEngine)Activator.CreateInstance(pluginAssembly.GetType("MusicBrowser.Interfaces.ICacheEngine"));
            }
            return null;
        }
    }
}
