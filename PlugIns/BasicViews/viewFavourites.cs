﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public class viewFavourites : IView
    {
        public string Title
        {
            get { return "Favourites"; }
        }

        public string Sort
        {
            get { return "[title]"; }
        }

        public bool SortAscending
        {
            get { return true; }
        }

        public EntityCollection Items
        {
            get
            {
                EntityCollection e = new EntityCollection();
                e.AddRange(Engines.Cache.InMemoryCache.GetInstance().DataSet
                    .Where(item => ((item.Kind == "Track") && (item.Rating >= 90) || (item.Loved)))
                    .Select(item => (Track)item)
                    .DedupeTracks()
                    .Select(item => (baseEntity)item)
                    .ToList());
                return e;
            }
        }
    }
}
