using System.Collections.Generic;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<baseEntity>
    {
        public void AddRange(IEnumerable<FileSystemItem> items)
        {
            foreach (FileSystemItem item in items)
            {
                baseEntity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                baseEntity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public new void Add(baseEntity e)
        {
            if (e != null)
            {
                base.Add(e);
            }
        }

        public void Add(IEnumerable<baseEntity> entities)
        {
            AddRange(entities);
        }

        public void Sort(string field)
        {
            string sort = field;
            if (sort.IndexOf(":sort") < 0)
            {
                sort = sort.Replace("]", ":sort]");
            }
            foreach (baseEntity e in this) 
            {
                e.SortName = e.TokenSubstitution(sort);
            }
            Sort(new EntityCollectionSorter());
        }

        protected static bool InheritsFrom<T>(baseEntity e)
        {
            return e is T;
        }

    }

    #region Folder Sorter

    sealed class EntityCollectionSorter : IComparer<baseEntity>
    {
        public int Compare(baseEntity x, baseEntity y)
        {
            return string.Compare(x.SortName, y.SortName, true);
        }
    }

    #endregion
}
