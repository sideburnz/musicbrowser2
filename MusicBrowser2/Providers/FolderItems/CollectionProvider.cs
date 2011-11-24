using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Util;

// gets the items from the collection's vf file

namespace MusicBrowser.Providers.FolderItems
{
    class CollectionProvider : IFolderItemsProvider
    {
        public IEnumerable<string> GetItems(string uri)
        {
            IFolderItemsProvider vf = new VirtualFolderProvider();
            string file = Path.Combine(Config.GetInstance().GetStringSetting("Collections.Folder"), uri);
            IEnumerable<string> collection = vf.GetItems(file);
            if (collection.Count() == 1)
            {
                IFolderItemsProvider f = new FolderProvider();
                return f.GetItems(collection.FirstOrDefault());
            }
            return collection;
        }
    }
}
