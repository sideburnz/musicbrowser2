using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Engines.Views
{
    public class Views
    {
        static Dictionary<string, IView> _views = new Dictionary<string, IView>();
        static Dictionary<string, IView> _kinds = new Dictionary<string, IView>();

        public static void RegisterView(IView view, string kind)
        {
            _views.Add(view.Title, view);
            _kinds.Add(kind + ":" + view.Title, view);
        }

        static public bool Exists(string title)
        {
            return _views.ContainsKey(title);
        }

        static public IView Fetch(string title)
        {
            if (title == "Recently Played") { return new MostRecentlyPlayed(); }
            return _views[title];
        }

        public static List<IView> GetViews(string kind)
        {
            List<IView> ret = new List<IView>();

            try
            {
                foreach (string item in _kinds.Keys)
                {
                    if (item.StartsWith(kind + ":"))
                    {
                        ret.Add(_kinds[item]);
                    }
                }
            }
            catch { }

            ret.Add(new MostRecentlyPlayed());

            return ret;
        }
    }
}
