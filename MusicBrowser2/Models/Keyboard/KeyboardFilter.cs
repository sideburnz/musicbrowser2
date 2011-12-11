using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Models.Keyboard
{
    class KeyboardFilter : IKeyboardHandler
    {
        public override void DoService()
        {
            EntityCollection res = new EntityCollection();
            foreach (baseEntity item in RawDataSet)
            {
                if (item.Title.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.Add(item);
                }
            }
            DataSet = res;
            Index = 0;
        }
    }
}
