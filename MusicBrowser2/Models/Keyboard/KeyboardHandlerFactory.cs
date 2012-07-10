using System.Collections.Generic;

namespace MusicBrowser.Models.Keyboard
{
    public class KeyboardHandlerFactory
    {
        public static List<string> Actions
        {
            get
            {
                return new List<string> { "Jump", "Search", "Filter" };
            }
        }

        public static IKeyboardHandler GetHandler()
        {
            switch (Util.Config.GetStringSetting("KeyboardAction").ToLower())
            {
                case "search":
                    return new KeyboardSearch();
                case "filter":
                    return new KeyboardFilter();
                default: // JIL
                    return new KeyboardJIL();
            }
        }
    }
}
