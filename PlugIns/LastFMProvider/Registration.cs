using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.PlugIns;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Engines.PlugIns.Actions;
using MusicBrowser.Providers.Metadata;
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
            Virtuals.Views.RegisterView(new Virtuals.viewPopular(), "MusicCollection");
        }
    }
}
