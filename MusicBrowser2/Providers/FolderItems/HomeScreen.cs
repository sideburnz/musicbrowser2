using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Util;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

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
                        baseEntity e = CollectionFactory(item);
                        if (e != null)
                        {
                            ret.Add(e);
                        }
                    }
                }
                return ret;
            }
        }

        public static baseEntity CollectionFactory(FileSystemItem vf)
        {
            baseEntity entity;
            string key = Helper.GetCacheKey(vf.FullPath);

            #region persistent cache
            // get the value from persistent cache
            entity = CacheEngineFactory.GetEngine().Fetch(key);
            if (entity == null)
            {
                string targetType = VirtualFolderProvider.GetTargetType(vf.FullPath);
                switch (targetType)
                {
                    case "music":
                        entity = new MusicCollection(); break;
                    case "video":
                        entity = new VideoCollection(); break;
                    case "photo":
                        entity = new PhotoCollection(); break;
                    default: // generic collection
                        entity = new Collection(); break;
                }
            }
            #endregion

            entity.Path = vf.FullPath;
            entity.ThumbPath = VirtualFolderProvider.GetImage(vf.FullPath);
            entity.Title = Path.GetFileNameWithoutExtension(vf.FullPath);

            return entity;
        }
    }
}
