using System.Collections.Generic;
using MusicBrowser.Providers;
using MusicBrowser.Entities.Kinds;

namespace MusicBrowser.Entities
{

    public class EntityCollection : List<IEntity>
    {
        public void Populate(IEnumerable<FileSystemItem> items, EntityFactory entityFactory, IEntity parent)
        {
            //this is a .Net3.5 app, .Net4 would allow for some parallelization, something like
            //Parallel.ForEach(IFolderItemsProvider.getItems(path), () => IEntityFactory.getItem(item));

            int duration = 0;
            int grandchildren = 0;

            foreach (FileSystemItem item in items)
            {
                IEntity entity = entityFactory.GetItem(item);

                if (entity.Kind.Equals(EntityKind.Unknown) || entity.Kind.Equals(EntityKind.Folder)) { continue; }

                entity.Parent = parent;
                entity.CalculateValues();
                Add(entity);
                
                if (entity.Duration > 0) { duration += entity.Duration; }
                if (entity.Children > 0) { grandchildren += entity.Children; }
            }

            if (parent.Children == 0) { parent.Children = Count; }
            if (parent.Duration == 0) { parent.Duration = duration; }
            if ((grandchildren > 0) && (parent.Kind.Equals(EntityKind.Artist))) { ((Artist)parent).GrandChildren = grandchildren; }

            parent.CalculateValues();

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
            bool xIsFolder = (x.Kind.Equals(EntityKind.Album)) || (x.Kind.Equals(EntityKind.Artist));
            bool yIsFolder = (y.Kind.Equals(EntityKind.Album)) || (y.Kind.Equals(EntityKind.Artist));

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
