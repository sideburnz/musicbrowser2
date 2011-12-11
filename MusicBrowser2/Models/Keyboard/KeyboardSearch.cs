using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Actions;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardSearch : IKeyboardHandler
    {
        public override void DoService()
        {
            if (String.IsNullOrEmpty(Value))
            {
                return;
            }
            ActionShowSearch action = new ActionShowSearch();
            action.SearchString = Value;
            action.Invoke();
            Value = String.Empty;
        }
    }
}
