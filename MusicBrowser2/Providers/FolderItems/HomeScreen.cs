using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.Providers.FolderItems
{
    class HomeScreen
    {
        public static EntityCollection Entities
        {
            get
            {
                EntityCollection ret = new EntityCollection();
                string path = Config.GetStringSetting("Collections.Folder");
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

        public static Collection CollectionFactory(FileSystemItem vf)
        {
            string key = Helper.GetCacheKey(vf.FullPath);

            #region persistent cache
            // get the value from persistent cache
            Collection entity = (Collection)CacheEngineFactory.GetEngine().Fetch(key);
            if (entity == null)
            {
                string targetType = VirtualFolderProvider.GetTargetType(vf.FullPath);
                switch (targetType.ToLower())
                {
                    case "music":
                        entity = new MusicCollection(); break;
                    case "video":
                        entity = new VideoCollection(); break;
                    default: // generic collection
                        entity = new Collection(); break;
                }
            }
            #endregion

            entity.Path = vf.FullPath;
            entity.ThumbPath = VirtualFolderProvider.GetImage(vf.FullPath);
            entity.Title = Path.GetFileNameWithoutExtension(vf.FullPath);
            entity.SortOrder = VirtualFolderProvider.GetSortOrder(vf.FullPath);

            return entity;
        }
    }
}
