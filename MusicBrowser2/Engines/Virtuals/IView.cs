using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Virtuals
{
    public interface IView
    {
        string Title { get; }
        EntityCollection Items { get; }
        string Sort { get; }
        bool SortAscending { get; }
    }

    public class MostRecentlyPlayed : IView
    {
        public string Title
        {
            get { return "MRU"; }
        }

        public string Sort
        {
            get { return "[lastplayed:sort]"; }
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
                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .OrderByDescending(item => item.LastPlayed)
                    .Where(item => item.LastPlayed > DateTime.MinValue) 
                    .Take(10)
                    .ToList());
                return e;
            }
        }
    }
}
