using System;
using System.Linq;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardJIL : IKeyboardHandler
    {
        public override void DoService()
        {
            int i = 0;
            foreach (baseEntity item in RawDataSet)
            {
                if (String.Compare(item.SortName, Value, true) > 0)
                {
                    Index = i;
                    return;
                }
                i++;
            }
            // if no match is found, go to the end of the list
            Index = RawDataSet.Count();
        }
    }
}
