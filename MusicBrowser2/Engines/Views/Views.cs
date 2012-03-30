using System.Collections.Generic;
using System.Linq;

namespace MusicBrowser.Engines.Views
{
    public class Views
    {
        static readonly Dictionary<string, IView> _views = new Dictionary<string, IView>();
        static readonly Dictionary<string, IView> Kinds = new Dictionary<string, IView>();

        public static void RegisterView(IView view, string kind)
        {
            _views.Add(view.Title, view);
            Kinds.Add(kind + ":" + view.Title, view);
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
                ret.AddRange(from item in Kinds.Keys where item.StartsWith(kind + ":") select Kinds[item]);
            }
            catch { }

            ret.Add(new MostRecentlyPlayed());

            return ret;
        }
    }
}
