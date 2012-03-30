using System.Collections.Generic;
using System;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<baseEntity>, IEnumerable<baseEntity>
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
            base.AddRange(entities);
        }

        public void Sort(string Field)
        {
            string sort = Field;
            if (sort.IndexOf(":sort") < 0)
            {
                sort = sort.Replace("]", ":sort]");
            }
            foreach (baseEntity e in this) 
            {
                e.SortName = e.TokenSubstitution(sort);
            }
            base.Sort(new EntityCollectionSorter());
        }

        protected static bool InheritsFrom<T>(baseEntity e)
        {
            return typeof(T).IsAssignableFrom(e.GetType());
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
