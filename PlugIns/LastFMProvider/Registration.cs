using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.PlugIns;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Engines.PlugIns.Actions;
using MusicBrowser.Providers.Metadata;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            // Actions
            Factory.RegisterAction(new ActionPlayPopularLastFM());
            Factory.RegisterAction(new ActionPlayRandomPopularLastFM());
            Factory.RegisterAction(new ActionPlaySimilarTracks());

            // Metadata
            Metadata.Providers.RegisterProvider(new LastFMMetadataProvider());
        }
    }
}
