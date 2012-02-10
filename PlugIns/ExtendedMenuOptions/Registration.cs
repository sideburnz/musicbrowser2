using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.PlugIns;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Engines.Metadata;
using MusicBrowser.Engines.PlugIns.Actions;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            // Actions
            Factory.RegisterAction(new ActionPlayFavourites(), "MusicCollection");
            Factory.RegisterAction(new ActionPlayMostPopular(), "MusicCollection");
            Factory.RegisterAction(new ActionPlayRandomPopular(), "MusicCollection");
            Factory.RegisterAction(new ActionPlayNewlyAdded(), "MusicCollection");
        }
    }
}
