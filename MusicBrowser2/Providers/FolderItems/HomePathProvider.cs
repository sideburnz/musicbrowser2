﻿using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Util;
using MusicBrowser.Entities;

namespace MusicBrowser.Providers.FolderItems
{
    class HomeScreen
    {
        public static EntityCollection Entities
        {
            get
            {
                EntityCollection ret = new EntityCollection();
                string path = Config.GetInstance().GetStringSetting("Collections.Folder");
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(path);
                
                foreach (FileSystemItem item in items)
                {
                    if (Path.GetExtension(item.Name).ToLower() == ".vf")
                    {
                        Entity e = new Entity();
                        e.Kind = EntityKind.Collection;
                        e.Path = item.FullPath;
                        e.Title = Path.GetFileNameWithoutExtension(item.FullPath);
                        //e.SortName = "";
                        //e.IconPath = "";
                        //TODO: get image path from the .vf
                        //TODO: get the sortorder from the .vf
                        ret.Add(e);
                    }
                }
                return ret;
            }
        }
    }
}
