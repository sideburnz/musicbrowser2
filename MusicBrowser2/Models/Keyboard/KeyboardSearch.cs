using System;
using MusicBrowser.Engines.Actions;

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
            ActionShowSearch action = new ActionShowSearch {SearchString = Value};
            action.Invoke();
            Value = String.Empty;
        }
    }
}
