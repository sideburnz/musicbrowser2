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
        private const string ThemeName = "White";

        public void Register()
        {
            Theme.SetScreen(ThemeScreens.Main, ThemeName, "resx://WhiteTheme/WhiteTheme.Resources/WhiteTheme");
            Theme.Views = new List<string>() { "list" };
        }
    }
}
