using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Util;
using System.IO;
using MusicBrowser.Providers.FolderItems;

namespace MusicBrowser
{
    static class FirstRun
    {

        public static void Initialize()
        {
            switch(Config.GetInstance().GetStringSetting("LastRunVersion"))
            {
                case "0.0.0.0":
                    LastVersion0000();
                    break;
            }
        }

        private static void LastVersion0000()
        {
                // delete actions.config
                string actionsFile = Path.Combine(Util.Helper.AppFolder, "actions.config");
                if (File.Exists(actionsFile))
                {
                    File.Delete(actionsFile);
                }

                // move the old library file, otherwise create one
                string libraryfile = Path.Combine(Util.Helper.AppFolder, "MusicLibrary.vf");
                string collectionfile = Path.Combine(Util.Helper.AppFolder, @"Collections\Music.vf");
                if (File.Exists(libraryfile) && !File.Exists(collectionfile))
                {
                    string targettype = VirtualFolderProvider.GetTargetType(libraryfile);
                    if (string.IsNullOrEmpty(targettype))
                    {
                        targettype = "music";
                    }

                    VirtualFolderProvider.WriteVF(collectionfile,
                        VirtualFolderProvider.GetFolders(libraryfile),
                        VirtualFolderProvider.GetLibraries(libraryfile),
                        VirtualFolderProvider.GetImage(libraryfile),
                        targettype,
                        VirtualFolderProvider.GetSortOrder(libraryfile));

                    File.Delete(libraryfile);
                }               

                Config.GetInstance().SetSetting("LastRunVersion", Application.Version);
        }

    }
}
