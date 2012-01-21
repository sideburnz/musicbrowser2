using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Models.Keyboard
{
    public class KeyboardHandlerFactory
    {
        public static List<string> Actions
        {
            get
            {
                return new List<string>() { "Jump", "Search", "Filter" };
            }
        }

        public static IKeyboardHandler GetHandler()
        {
            switch (Util.Config.GetInstance().GetStringSetting("KeyboardAction").ToLower())
            {
                case "search":
                    return new KeyboardSearch();
                case "filter":
                    return new KeyboardFilter();
                case "jump":
                default: // JIL
                    return new KeyboardJIL();
            }
        }
    }
}
