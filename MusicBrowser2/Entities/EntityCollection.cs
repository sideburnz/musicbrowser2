using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<IEntity>
    {
        public void Populate(IEnumerable<FileSystemItem> items)
        {
            NearLineCache nl = NearLineCache.GetInstance();

            foreach (FileSystemItem item in items)
            {
                //Logging.Logger.Debug("EntityCollection.Loop (" + item.FullPath);

                IEntity entity = EntityFactory.GetItem(item);
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

    sealed class EntityCollectionSorter : IComparer<IEntity>
    {
        public int Compare(IEntity x, IEntity y)
        {
            bool xIsFolder = (x.Kind.Equals(EntityKind.Album)) || 
                (x.Kind.Equals(EntityKind.Artist) || 
                (x.Kind.Equals(EntityKind.Genre)));
            bool yIsFolder = (y.Kind.Equals(EntityKind.Album)) || 
                (y.Kind.Equals(EntityKind.Artist) || 
                (y.Kind.Equals(EntityKind.Genre)));

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
