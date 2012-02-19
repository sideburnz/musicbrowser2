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
                int playlistsize = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");

                EntityCollection e = new EntityCollection();
                e.AddRange(Engines.Cache.InMemoryCache.GetInstance().DataSet
                    .Where(item => item.Kind == "Track" && item.TimesPlayed > 0)
                    .OrderByDescending(item => item.TimesPlayed)
                    .Take(playlistsize)
                    .ToList());
                return e;
            }
        }
    }
}
