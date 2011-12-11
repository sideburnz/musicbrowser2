using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardJIL : IKeyboardHandler
    {
        public override void DoService()
        {
            foreach (baseEntity item in RawDataSet)
            {
                if (item.Title.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    Index = item.Index;
                    break;
                }
            }
        }
    }
}
