using System;
using System.Collections.Generic;
using MusicBrowser.Providers.FolderItems;

namespace MusicBrowser.Entities.Kinds
{
    public class Home : IEntity
    {
        static IEnumerable<string> _paths;

        public Home()
        {
            Path = "home";
            Title = "MusicBrowser2";
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2";
            string temp = Util.Config.GetInstance().GetSetting("HomeBackground");
            if (System.IO.File.Exists(temp)) { DefaultBackgroundPath = temp; }
            base.Parent = new Unknown();
            base.UpdateValues();
        }

        public override EntityKind Kind 
        {
            get { return EntityKind.Home; }
        }

        public override string View
        {
            get { return Util.Config.HandleEntityView(EntityKind.Home).ToLower(); }
        }

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
                        VirtualFolderProvider.WriteVF(Util.Config.GetInstance().GetSetting("ManualLibraryFile"), _paths);
                    }
                    else
                    {
                        string vfFile = Util.Config.GetInstance().GetSetting("ManualLibraryFile");
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
