using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Providers.FolderItems
{
    class HomePathProvider
    {
        static IEnumerable<string> _paths = null;

        public static IEnumerable<string> Paths
        {
            get
            {
                if (_paths == null)
                {
                    _paths = new List<string>();
                    bool runningOnExtender = Environment.UserName.ToLower().StartsWith("mcx");

                    if ((Util.Config.GetInstance().GetBooleanSetting("WindowsLibrarySupport")) && !runningOnExtender)
                    {
                        IFolderItemsProvider folderItemsProvider = new WindowsLibraryProvider();
                        _paths = folderItemsProvider.GetItems("music");
                        VirtualFolderProvider.WriteVF(Path.Combine(Util.Helper.AppFolder, "Extender.vf"), _paths);
                        return _paths;
                    }

                    string vfFile;
                    if (runningOnExtender)
                    {
                        vfFile = Path.Combine(Util.Helper.AppFolder, "Extender.vf");
                    }
                    else
                    {
                        vfFile = Util.Config.GetInstance().GetStringSetting("ManualLibraryFile");
                    }

                    if (File.Exists(vfFile))
                    {
                        IFolderItemsProvider folderItemsProvider = new VirtualFolderProvider();
                        _paths = folderItemsProvider.GetItems(vfFile);
                    }
                    else
                    {
                        Exception ex = new Exception("Virtual Folder " + vfFile + " not found");
                        LoggerEngineFactory.Error(ex);
                        throw ex;
                    }
                }
                return _paths;
            }
        }
    }
}
