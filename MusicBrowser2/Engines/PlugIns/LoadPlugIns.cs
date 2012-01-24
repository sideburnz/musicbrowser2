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
            string libraryFolder = Util.Helper.PlugInFolder;

            foreach (FileSystemItem item in FileSystemProvider.GetFolderContents(libraryFolder))
            {
                if (item.Name.ToLower().EndsWith(".dll"))
                {
                    string pluginfile = Path.Combine(@"C:\Windows\eHome", item.Name);
                    try
                    {
                        File.Copy(item.FullPath, pluginfile, true);
                        string debug = item.FullPath.Replace(".dll", ".pdb");
                        if (File.Exists(debug))
                        {
                            File.Copy(debug, pluginfile.Replace(".dll", ".pdb"), true);
                        }
                    }
                    catch { }
                    if (File.Exists(pluginfile))
                    {
                        Logging.LoggerEngineFactory.Info("Loading " + item.Name);
                        LoadExternalEngine(pluginfile);
                    }
                }
            }
        }

        private static void LoadExternalEngine(string libraryPath)
        {
            try
            {
                //Assembly pluginAssembly = Assembly.LoadFile(libraryPath);
                Assembly pluginAssembly = Assembly.Load(System.IO.File.ReadAllBytes(libraryPath));
                IPlugIn plugin = (IPlugIn)Activator.CreateInstance(pluginAssembly.GetType("MusicBrowser.Engines.PlugIns.Registration"));
                plugin.Register();
            }
            catch (Exception e)
            {
                Logging.LoggerEngineFactory.Error(e);
            }
        }
    }
}
