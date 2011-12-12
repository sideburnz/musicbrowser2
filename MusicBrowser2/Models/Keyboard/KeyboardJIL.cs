using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardJIL : IKeyboardHandler
    {
        private static IEnumerable<string> _sortIgnore = Util.Config.GetInstance().GetListSetting("SortReplaceWords");

        private static string HandleIgnoreWords(string value)
        {
            foreach (string item in _sortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1).Replace(" ", ""); }
            }
            return value.Replace(" ", "");
        }


        public override void DoService()
        {
            foreach (baseEntity item in RawDataSet)
            {
                string candidate = HandleIgnoreWords(item.Title.ToLower());

                if (String.Compare(candidate, Value, true) > 0)
                {
                    Index = item.Index;
                    return;
                }
            }
            Index = RawDataSet.Count();
        }
    }
}
