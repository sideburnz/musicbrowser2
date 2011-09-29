using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                    if ((Util.Config.GetInstance().GetBooleanSetting("WindowsLibrarySupport")) && !(Environment.UserName.ToLower().StartsWith("mcx")))
                    {
                        IFolderItemsProvider folderItemsProvider = new WindowsLibraryProvider();
                        _paths = folderItemsProvider.GetItems("music");
                        VirtualFolderProvider.WriteVF(Util.Config.GetInstance().GetStringSetting("ManualLibraryFile"), _paths);
                    }
                    else
                    {
                        string vfFile = Util.Config.GetInstance().GetStringSetting("ManualLibraryFile");
                        if (System.IO.File.Exists(vfFile))
                        {
                            IFolderItemsProvider folderItemsProvider = new VirtualFolderProvider();
                            _paths = folderItemsProvider.GetItems(vfFile);
                        }
                        else
                        {
                            Exception ex = new Exception("Virtual Folder " + vfFile + " not found");
                            Logging.Logger.Error(ex);
                            throw ex;
                        }
                    }
                }
                return _paths;
            }
        }
    }
}
