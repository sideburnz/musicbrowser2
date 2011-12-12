using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Models.Keyboard
{
    static class KeyboardHandlerFactory
    {
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
