using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardFilter : IKeyboardHandler
    {
        private bool isMatch(string candidate, string criteria)
        {
            candidate = candidate.ToLower().Trim();
            List<string> candidates = new List<string>();
            candidates.Add(candidate.Replace(" ", ""));
            if (candidate.StartsWith("the "))
            {
                candidates.Add(candidate.Substring(4));
            }
            foreach (string c in candidates)
            {
                if (c.StartsWith(criteria)) { return true; }
            }
            return false;
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
                Value = Value.Substring(0, Value.Length - 1);
                return;
            }
            DataSet = res;
            Index = 0;
        }
    }
}
