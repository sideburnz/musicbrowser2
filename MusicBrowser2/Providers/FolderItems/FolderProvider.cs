using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Providers.FolderItems
{
    class FolderProvider : IFolderItemsProvider
    {
        public IEnumerable<string> GetItems(string uri)
        {
            foreach (FileSystemItem item in FileSystemProvider.GetFolderContents(uri))
            {
                yield return item.FullPath;
            }
        }
    }
}
