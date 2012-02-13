using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Engines.Virtuals
{
    static public class Views
    {
        static Dictionary<string, IView> _views = new Dictionary<string, IView>();

        static public void RegisterView(IView view)
        {
            _views.Add(view.Title, view);
        }

        static public bool Exists(string title)
        {
            return _views.ContainsKey(title);
        }

        static public IView Fetch(string title)
        {
            return _views[title];
        }
    }
}
