using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public interface IView
    {
        string Title { get; }
        EntityCollection Items { get; }
        string Sort { get; }
        bool SortAscending { get; }
    }
}
