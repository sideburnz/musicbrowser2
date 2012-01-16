using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Engines.Themes
{
    public enum ThemeScreens
    {
        Main,
        Search,
        FooPlaying
    }

    public static class Theme
    {
        private static string _mainscreen = "resx://MusicBrowser/MusicBrowser.Resources/pageFolder";
        private static string _searchscreen = "resx://MusicBrowser/MusicBrowser.Resources/pageSearch";
        private static string _fooplaying = "resx://MusicBrowser/MusicBrowser.Resources/pageFooBar2000";
        private static List<string> _availableViews = new List<string>() { "list", "thumb", "strip" };

        public static void SetScreen(ThemeScreens screen, string theme, string URL)
        {
            // test that this is the current theme
            if (theme.ToLower() != Util.Config.GetInstance().GetStringSetting("Theme").ToLower()) { return; }

            Logging.LoggerEngineFactory.Info(String.Format("Loading {0} screen from {1} theme", screen.ToString(), theme));

            switch (screen)
            {
                case ThemeScreens.Main:
                    {
                        Main = URL;
                        break;
                    }
                case ThemeScreens.FooPlaying:
                    {
                        FooPlaying = URL;
                        break;
                    }
                case ThemeScreens.Search:
                    {
                        Search = URL;
                        break;
                    }
            }
        }

        public static string Main 
        { 
            get
            {
                return _mainscreen;
            }
            private set
            {
                _mainscreen = value;
            } 
        }

        public static string FooPlaying
        {
            get
            {
                return _fooplaying;
            }
            private set
            {
                _fooplaying = value;
            }
        }

        public static string Search
        {
            get
            {
                return _searchscreen;
            }
            private set
            {
                _searchscreen = value;
            }
        }

        public static List<String> Views
        {
            get
            {
                return _availableViews;
            }
            set
            {
                _availableViews.Clear();
                _availableViews.AddRange(value);
            }
        }
    }
}
