using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            int count = collection.Count();
            if (count == 1)
            {
                IFolderItemsProvider f = new FolderProvider();
                string first = collection.FirstOrDefault();
                return f.GetItems(first);
            }
            return collection;
        }
    }
}
