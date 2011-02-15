using System.Collections.Generic;
using System.Linq;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<IEntity>
    {
        public void Populate(IEnumerable<FileSystemItem> items, IEntityFactory entityFactory, IEntity parent)
        {
            //this is a .Net3.5 app, .Net4 would allow for some parallelization, something like
            //Parallel.ForEach(IFolderItemsProvider.getItems(path), () => IEntityFactory.getItem(item));

            foreach (FileSystemItem item in items)
            {
                if (item.Name.ToLower() != "metadata")
                {
                    IEntity entity = entityFactory.getItem(item);
                    entity.Parent = parent;
                    if (!entity.Kind.Equals(EntityKind.Unknown))
                    {
                        entity.CalculateValues();
                        this.Add(entity);
                    }
                }
                parent.Children = this.Count;
            }
            parent.CalculateValues();

            this.Sort(new EntityCollectionSorter());
            this.IndexItems();
        }

        public void IndexItems()
        {
            for (int i = 0; i < this.Count; i++)
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
            bool xIsFolder = (x.Kind == EntityKind.Album) || (x.Kind == EntityKind.Artist);
            bool yIsFolder = (y.Kind == EntityKind.Album) || (y.Kind == EntityKind.Artist);

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
