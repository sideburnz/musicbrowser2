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
                baseEntity entity = Factory.GetItem(item);
                Add(entity);
            }
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                baseEntity entity = Factory.GetItem(item);
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

        public void Sort(string field)
        {
            string sort = field;
            if (sort.IndexOf(":sort", System.StringComparison.Ordinal) < 0)
            {
                sort = sort.Replace("]", ":sort]");
            }
            foreach (baseEntity e in this) 
            {
                e.SortName = e.TokenSubstitution(sort);
            }
            Sort(new EntityCollectionSorter());
        }
    }
}
