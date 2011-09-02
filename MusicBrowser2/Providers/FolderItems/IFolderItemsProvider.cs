using System.Collections.Generic;

namespace MusicBrowser.Providers.FolderItems
{
    /// <summary>
    /// Interface for classes which get lists of files, directories and URLs.
    /// </summary>
    public interface IFolderItemsProvider
    {
        IEnumerable<string> GetItems(string uri);
    }
}
