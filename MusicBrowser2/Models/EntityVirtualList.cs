using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{  
    public class EntityVirtualList : VirtualList
    {
        private readonly EntityCollection _collection;

        public EntityVirtualList(EntityCollection entityCollection, string field, bool ascending)
        {
            entityCollection.Sort(field);
            if (!ascending) { entityCollection.Reverse(); }
            _collection = entityCollection;
            Count = _collection.Count;
            EnableSlowDataRequests = false;  
        }

        // this allows the page to load without
        protected override void OnRequestItem(int index, ItemRequestCallback callback)
        {
            callback(this, index, _collection[index]);
        }

    }
}