using System.Collections.Generic;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardFilter : IKeyboardHandler
    {
        private static readonly IEnumerable<string> SortIgnore = Util.Config.GetListSetting("SortReplaceWords");

        private static string HandleIgnoreWords(string value)
        {
            foreach (string item in SortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1).Replace(" ", ""); }
            }
            return value.Replace(" ", "");
        }

        private bool isMatch(string candidate, string criteria)
        {
            candidate = candidate.ToLower().Trim();
            return ((candidate.StartsWith(criteria)) || (HandleIgnoreWords(candidate).StartsWith(criteria)));
        }

        public override void DoService()
        {
            EntityCollection res = new EntityCollection();
            foreach (baseEntity item in RawDataSet)
            {
                if (isMatch(item.Title, Value))
                {
                    res.Add(item);
                }
            }
            if (res.Count == 0)
            {
                // prevent filtering everything
                return;
            }
            DataSet = res;
            Index = 0;
        }
    }
}
