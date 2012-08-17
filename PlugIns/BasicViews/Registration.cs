using MusicBrowser.Engines.Views;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            Views.Views.RegisterView(new viewFavourites(), "MusicCollection");
            Views.Views.RegisterView(new viewMostPlayed(), "MusicCollection");
            Views.Views.RegisterView(new viewRecentlyAddedMusic(), "MusicCollection");
            Views.Views.RegisterView(new ViewRandomPopular(), "MusicCollection");

            Views.Views.RegisterView(new viewRecentlyAddedVideo(), "VideoCollection");

            Views.Views.RegisterView(new viewRecentlyAdded(), "Home");
        }
    }
}
