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
                Entity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public void IndexItems()
        {
            for (int i = 0; i < Count; i++) 
            {
                this[i].Index = i; 
            }
        }

        public new void Add(Entity e)
        {
            if (e != null) 
            {
                e.UpdateValues();
                base.Add(e); 
            }
        }

        public new void Sort()
        {
            base.Sort(new EntityCollectionSorter());
        }
    }

    #region Folder Sorter

    sealed class EntityCollectionSorter : IComparer<Entity>
    {
        public int Compare(Entity x, Entity y)
        {
            bool xIsItem = x.Playable;
            bool yIsItem = y.Playable;

            // folders (artists and albums) have a higher priority than playlists and songs
            if (!xIsItem && yIsItem) { return -1; }
            if (xIsItem && !yIsItem) { return 1; }

            return string.Compare(x.SortName, y.SortName, true);
        }
    }

    #endregion
}
