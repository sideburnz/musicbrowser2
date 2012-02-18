using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Virtuals
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
                EntityCollection e = new EntityCollection();
                DateTime epoch = DateTime.Parse("2000-JAN-01");
                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .OrderByDescending(item => item.LastPlayed)
                    .Where(item => item.LastPlayed > epoch) 
                    .Take(10)
                    .ToList());
                return e;
            }
        }
    }
}
