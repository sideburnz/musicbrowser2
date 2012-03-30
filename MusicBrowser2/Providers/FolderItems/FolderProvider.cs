using System.Collections.Generic;
using System.Linq;

namespace MusicBrowser.Providers.FolderItems
{
    class FolderProvider : IFolderItemsProvider
    {
        public IEnumerable<string> GetItems(string uri)
        {
            return FileSystemProvider.GetFolderContents(uri).Select(item => item.FullPath);
        }
    }
}
