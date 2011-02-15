using System.Collections.Generic;
using MusicBrowser.Providers;

namespace MusicBrowser.Providers.FolderItems
{
    /// <summary>
    /// Interface for classes which get lists of files, directories and URLs.
    /// </summary>
    public interface IFolderItemsProvider
    {
        IEnumerable<string> getItems(string URI);
    }
}
