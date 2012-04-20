// seperates UI specific view information from entities
// also allows view state information to be cached and 
// shared seperate to the main cache

namespace MusicBrowser.Engines.ViewState
{
    public interface IViewState
    {
        // read only attributes
        string View { get; }
        string SortField { get; }
        bool SortAscending { get; }
        int ThumbSize { get; }

        // accessors
        void SetThumbSize(int size);
        void SetSortField(string field);
        void InvertSort();
        void SetView(string view);

        string DefaultSort { get; set; }
        string DefaultView { get; set; }
    }
}
