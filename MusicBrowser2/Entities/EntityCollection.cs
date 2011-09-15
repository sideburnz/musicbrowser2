using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<Entity>
    {
        public void Populate(IEnumerable<FileSystemItem> items)
        {
            NearLineCache nl = NearLineCache.GetInstance();

            foreach (FileSystemItem item in items)
            {
                //Logging.Logger.Debug("EntityCollection.Loop (" + item.FullPath);

                Entity entity = EntityFactory.GetItem(item);
                if (!(entity == null) && !entity.Kind.Equals(EntityKind.Folder))
                {
                    entity.UpdateValues();
                    Add(entity);
                }
            }
            Sort(new EntityCollectionSorter());
            IndexItems();
        }

        public void IndexItems()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Index = i;
            }
        }
    }

    #region Folder Sorter

    sealed class EntityCollectionSorter : IComparer<Entity>
    {
        public int Compare(Entity x, Entity y)
        {
            bool xIsFolder = !x.Playable;
            bool yIsFolder = !y.Playable;

            // folders (artists and albums) have a higher priority than playlists and songs
            if (xIsFolder && !(yIsFolder))
            {
                return -1;
            }
            if (!(xIsFolder) && yIsFolder)
            {
                return 1;
            }

            return string.Compare(x.SortName, y.SortName, true);
        }
    }

    #endregion
}
