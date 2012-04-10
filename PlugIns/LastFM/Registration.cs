using MusicBrowser.Engines.Metadata;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            // Actions
            //Factory.RegisterAction(new ActionPlaySimilarTracks(), "MusicCollection");
            //Factory.RegisterAction(new ActionPlayForgottenTracks(), "MusicCollection");

            // Metadata
            Metadata.Providers.RegisterProvider(new LastFMArtistMetadataProvider());
            Metadata.Providers.RegisterProvider(new LastFMAlbumMetadataProvider());
            Metadata.Providers.RegisterProvider(new LastFMTrackMetadataProvider());

            //Views
            Views.Views.RegisterView(new Views.viewPopular(), "MusicCollection");
        }
    }
}
