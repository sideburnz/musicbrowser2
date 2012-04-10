using System.Collections.Generic;
using System;

namespace MusicBrowser.Entities
{
    sealed class EntityCollectionSorter : IComparer<baseEntity>
    {
        public int Compare(baseEntity x, baseEntity y)
        {
            return String.Compare(x.SortName, y.SortName, StringComparison.OrdinalIgnoreCase);
        }
    }
}