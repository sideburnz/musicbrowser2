using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Util;
using System.IO;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers;

namespace MusicBrowser
{
    static class FirstRun
    {

        private static string collectionPath = Path.Combine(Util.Helper.AppFolder, @"Collections");

        public static void Initialize()
        {
            Directory.CreateDirectory(collectionPath);

            switch(Config.GetInstance().GetStringSetting("LastRunVersion"))
            {
                case "0.0.0.0":
                    LastVersion0000();
                    break;
            }

            if (!CollectionExists())
            {
                CreateCollection();
            }
        }

        private static bool CollectionExists()
        {
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(collectionPath);
            foreach(FileSystemItem item in items)
            {
                if (Path.GetExtension(item.Name).ToLower() == ".vf")
                {
                    return true;
                }
            }
            return false;
        }

        private static void CreateCollection()
        {
            string collectionfile = Path.Combine(collectionPath, "Music.vf");

            VirtualFolderProvider.WriteVF(collectionfile,
                new List<string>(),
                new List<string>() { "music" },
                String.Empty,
                "music",
                "000");
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
                string collectionfile = Path.Combine(collectionPath, "Music.vf");
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
