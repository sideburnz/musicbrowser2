using System;
using System.Linq;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public class MostRecentlyPlayed : IView
    {
        public string Title
        {
            get { return "Recently Played"; }
        }

        public string Sort
        {
            get { return "[lastplayed]"; }
        }

        public bool SortAscending
        {
            get { return false; }
        }

        public EntityCollection Items
        {
            get 
            {
                int playlistsize = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
                EntityCollection e = new EntityCollection();
                DateTime epoch = DateTime.Parse("2000-JAN-01");

                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .OrderByDescending(item => item.LastPlayed)
                    .Where(item => item.LastPlayed > epoch)
                    .Take(playlistsize)
                    .ToList());
                return e;
            }
        }
    }
}
