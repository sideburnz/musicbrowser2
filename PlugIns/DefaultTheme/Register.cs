using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.PlugIns;
using MusicBrowser.Engines.Themes;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            Theme.SetScreen(ThemeScreens.Main, "default", "resx://DefaultTheme/DefaultTheme.Resources/pageFolder");
        }
    }
}
