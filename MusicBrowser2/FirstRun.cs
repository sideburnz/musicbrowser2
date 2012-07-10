using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Util;

namespace MusicBrowser
{
    static class FirstRun
    {

        private static readonly string CollectionPath = Path.Combine(Config.AppFolder, @"Collections");

        public static void Initialize()
        {
            Directory.CreateDirectory(CollectionPath);

            switch(Config.GetStringSetting("LastRunVersion"))
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
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(CollectionPath);
            return items.Any(item => Path.GetExtension(item.Name).ToLower() == ".vf");
        }

        private static void CreateCollection()
        {
            string collectionfile = Path.Combine(CollectionPath, "Music.vf");

            VirtualFolderProvider.WriteVF(collectionfile,
                new List<string>(),
                new List<string> { "music" },
                String.Empty,
                "music",
                "000");
        }

        private static void LastVersion0000()
        {
                // delete actions.config
                string actionsFile = Path.Combine(Config.AppFolder, "actions.config");
                if (File.Exists(actionsFile))
                {
                    File.Delete(actionsFile);
                }

                // move the old library file, otherwise create one
                string libraryfile = Path.Combine(Config.AppFolder, "MusicLibrary.vf");
                string collectionfile = Path.Combine(CollectionPath, "Music.vf");
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

                Config.SetSetting("LastRunVersion", Application.Version);
        }

    }
}
