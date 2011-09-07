using System;
using System.Collections.Generic;
using MusicBrowser.Providers.FolderItems;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities.Kinds
{
    [DataContract]
    [KnownType(typeof(Home))]
    public class Home : IEntity
    {
        static IEnumerable<string> _paths;

        public Home()
        {
            Path = "home";
            Title = "MusicBrowser2";
        }

        public override string DefaultIconPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2"; }
        }

        public override string DefaultBackgroundPath
        {
            get
            {
                string temp = Util.Config.GetInstance().GetSetting("HomeBackground");
                if (System.IO.File.Exists(temp)) 
                { 
                    return temp; 
                }
                return base.DefaultBackgroundPath;
            }
        }

        public override EntityKind Kind 
        {
            get { return EntityKind.Home; }
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
