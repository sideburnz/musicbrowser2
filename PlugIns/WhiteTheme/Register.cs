using System.Collections.Generic;
using MusicBrowser.Engines.Themes;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        private const string ThemeName = "White";

        public void Register()
        {
            ThemeLoader.SetScreen(ThemeScreens.Main, ThemeName, "resx://WhiteTheme/WhiteTheme.Resources/WhiteTheme");
            ThemeLoader.SetLayouts(ThemeName, new List<string>() { "List" });
            ThemeLoader.AddTheme("White");
        }
    }
}
