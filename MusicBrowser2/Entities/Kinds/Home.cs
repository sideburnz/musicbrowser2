using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Providers.FolderItems;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Entities.Kinds
{
    class Home : IEntity
    {
        public Home()
        {
            Title = "MusicBrowser2";
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2";
            string temp = Util.Config.getInstance().getSetting("HomeBackground");
            if (System.IO.File.Exists(temp)) { DefaultBackgroundPath = temp; }
            Parent = new Unknown();
            CalculateValues();
        }

        public override EntityKind Kind 
        {
            get { return EntityKind.Home; }
        }

        public override string View
        {
            get { return Util.Config.handleEntityView(EntityKind.Home).ToLower(); }
        }

        public IEnumerable<string> Paths
        {
            get 
            {
                IEnumerable<string> homePaths = new List<string>();

                if ((Util.Config.getInstance().getBooleanSetting("WindowsLibrarySupport")) && !(Environment.UserName.ToLower().StartsWith("mcx")))
                {
                    IFolderItemsProvider folderItemsProvider;
                    folderItemsProvider = new WindowsLibraryProvider();
                    homePaths = folderItemsProvider.getItems("music");
                    VirtualFolderProvider.WriteVF(Util.Config.getInstance().getSetting("ManualLibraryFile"), homePaths);
                }
                else
                {
                    string vfFile = Util.Config.getInstance().getSetting("ManualLibraryFile");
                    if (System.IO.File.Exists(vfFile))
                    {
                        IFolderItemsProvider folderItemsProvider;
                        folderItemsProvider = new VirtualFolderProvider();
                        homePaths = folderItemsProvider.getItems(vfFile);
                    }
                    else
                    {
                        Exception ex = new Exception("Virtual Folder " + vfFile + " not found");
                        Logging.Logger.Error(ex);
                        throw ex;
                    }
                }
                return homePaths;
            }
        }
    }
}
