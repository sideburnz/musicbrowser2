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

                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .Where(item => (item.Playable) && (item.InheritsFrom<Item>()) && (item.PlayState.Played))
                    .OrderByDescending(item => item.PlayState.LastPlayed)
                    .Take(playlistsize)
                    .ToList());
                return e;
            }
        }
    }
}
