﻿using System.Linq;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public class viewPopular : IView
    {
        public string Title
        {
            get { return "Most Played on Last.fm"; }
        }

        public string Sort
        {
            get { return "[lastfmplaycount]"; }
        }

        public bool SortAscending
        {
            get { return false; }
        }

        public EntityCollection Items
        {
            get
            {
                int playlistsize = Util.Config.GetIntSetting("AutoPlaylistSize");

                EntityCollection e = new EntityCollection();
                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .Where(item => item.Kind == "Track")
                    .Select(item => (Track)item)
                    .DedupeTracks()
                    .OrderByDescending(item => item.LastFMPlayCount)
                    .Take(playlistsize)
                    .Select(item => (baseEntity)item)
                    .ToList());

                return e;
            }
        }
    }
}
