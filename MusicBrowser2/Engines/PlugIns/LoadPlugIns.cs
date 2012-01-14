using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Reflection;
using MusicBrowser.Providers;

namespace MusicBrowser.Engines.PlugIns
{
    static class LoadPlugIns
    {
        public static void Execute()
        {
            string libraryFolder = Path.Combine(Util.Helper.PlugInFolder, "Providers");

            foreach (FileSystemItem item in FileSystemProvider.GetFolderContents(libraryFolder))
            {
                if (item.Name.ToLower().EndsWith(".dll"))
                {
                    LoadExternalEngine(item.FullPath);
                }
            }
        }

        private static void LoadExternalEngine(string libraryPath)
        {
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(libraryPath);
                IPlugIn plugin = (IPlugIn)Activator.CreateInstance(pluginAssembly.GetType("MusicBrowser.Engines.PlugIns.Registration"));
                plugin.Register();
            }
            catch (Exception e)
            {
                Logging.LoggerEngineFactory.Error(e);
                throw (e);
            }
        }
    }
}
