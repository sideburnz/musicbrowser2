using System;
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
                        baseEntity e;
                        switch (VirtualFolderProvider.GetTargetType(item.FullPath))
                        {
                            case "music":
                                e = new MusicCollection(); break;
                            case "video":
                                e = new VideoCollection(); break;
                            case "photo":
                                e = new PhotoCollection(); break;
                            default: // generic collection
                                e = new Collection(); break;

                        }

                        e.Path = item.FullPath;
                        e.ThumbPath = VirtualFolderProvider.GetImage(item.FullPath);
                        e.Title = Path.GetFileNameWithoutExtension(item.FullPath);
                        ret.Add(e);
                    }
                }
                return ret;
            }
        }
    }
}
