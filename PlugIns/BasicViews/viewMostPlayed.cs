using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public class viewMostPlayed : IView
    {
        public string Title
        {
            get { return "Most Played"; }
        }

        public string Sort
        {
            get { return "[timesplayed]"; }
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
                    .Where(item => item.Kind == "Track" && item.PlayState.Played)
                    .OrderByDescending(item => item.PlayState.TimesPlayed)
                    .Select(item => (Track)item)
                    .DedupeTracks()
                    .Take(playlistsize)
                    .Select(item => (baseEntity)item)
                    .ToList());
                return e;
            }
        }
    }
}
