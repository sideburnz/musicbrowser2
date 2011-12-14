using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Models
{  
    public class EntityVirtualList : VirtualList
    {
        private EntityCollection _collection;

        public EntityVirtualList(EntityCollection entityCollection, string field)
        {
            entityCollection.Sort(field);
            _collection = entityCollection;
            this.Count = _collection.Count;
            this.EnableSlowDataRequests = false;  
        }

        // this allows the page to load without
        protected override void OnRequestItem(int index, ItemRequestCallback callback)
        {
            callback(this, index, _collection[index]);
        }

    }
}