using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Reflection;

namespace MusicBrowser.Engines.Themes
{
    static class ThemeFactory
    {
        private static ITheme _theme;
        private static readonly object Obj = new object();

        public static ITheme GetTheme()
        {
            if (_theme == null)
            {
                string libraryName = Util.Config.GetInstance().GetSetting("Theme");
                lock (Obj)
                {
                    _theme = LoadExternalEngine(libraryName);
                }
            }
            return _theme;
        }

        public static ITheme LoadExternalEngine(string theme)
        {
            string libraryFolder = Path.Combine(Util.Helper.PlugInFolder, "Themes");
            string libraryPath = Path.Combine(libraryFolder, theme + ".dll");

            if (File.Exists(libraryPath))
            {
                try
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(libraryPath);
                    return (ITheme)Activator.CreateInstance(pluginAssembly.GetType("MusicBrowser.Engines.Themes.Theme"));
                }
                catch(Exception e)
                {
                    Logging.LoggerEngineFactory.Error(e);
                    throw (e);
                }
            }
            return null;
        }
    }
}