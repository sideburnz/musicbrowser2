using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Views
{
    public class viewRecentlyAddedMusic : IView
    {
        public string Title
        {
            get { return "Recently Added Music"; }
        }

        public string Sort
        {
            get { return "[added]"; }
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
                    .Where(item => item.Kind == "Track")
                    .OrderByDescending(item => item.TimeStamp)
                    .Take(playlistsize)
                    .ToList());

                return e;
            }
        }
    }
}
