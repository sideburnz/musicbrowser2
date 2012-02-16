using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Virtuals;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            Views.RegisterView(new viewFavourites(), "MusicCollection");
            Views.RegisterView(new viewMostPlayed(), "MusicCollection");
            Views.RegisterView(new viewRecentlyAddedMusic(), "MusicCollection");

            Views.RegisterView(new viewRecentlyAddedVideo(), "VideoCollection");

            Views.RegisterView(new viewRecentlyAdded(), "Home");
        }
    }
}
