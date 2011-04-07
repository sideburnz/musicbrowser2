﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Providers.FolderItems;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Entities.Kinds
{
    public class Home : IEntity
    {
        static IEnumerable<string> _paths;

        public Home()
        {
            base.Title = "MusicBrowser2";
            base.DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2";
            string temp = Util.Config.getInstance().getSetting("HomeBackground");
            if (System.IO.File.Exists(temp)) { base.DefaultBackgroundPath = temp; }
            base.Parent = new Unknown();
            base.CalculateValues();
        }

        public override EntityKind Kind 
        {
            get { return EntityKind.Home; }
        }

        public override string View
        {
            get { return Util.Config.handleEntityView(EntityKind.Home).ToLower(); }
        }

        public static IEnumerable<string> Paths
        {
            get 
            {
                if (_paths == null)
                {
                    _paths = new List<string>();

                    if ((Util.Config.getInstance().getBooleanSetting("WindowsLibrarySupport")) && !(Environment.UserName.ToLower().StartsWith("mcx")))
                    {
                        IFolderItemsProvider folderItemsProvider;
                        folderItemsProvider = new WindowsLibraryProvider();
                        _paths = folderItemsProvider.getItems("music");
                        VirtualFolderProvider.WriteVF(Util.Config.getInstance().getSetting("ManualLibraryFile"), _paths);
                    }
                    else
                    {
                        string vfFile = Util.Config.getInstance().getSetting("ManualLibraryFile");
                        if (System.IO.File.Exists(vfFile))
                        {
                            IFolderItemsProvider folderItemsProvider;
                            folderItemsProvider = new VirtualFolderProvider();
                            _paths = folderItemsProvider.getItems(vfFile);
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
