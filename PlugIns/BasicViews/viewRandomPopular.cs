using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Views
{
    public class ViewRandomPopular : IView
    {
        public string Title
        {
            get { return "Random Most Played"; }
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
                int playlistsize = Config.GetIntSetting("AutoPlaylistSize");

                var e = new EntityCollection();
                e.AddRange(Cache.InMemoryCache.GetInstance().DataSet
                    .Where(item => item.Kind == "Track" && item.PlayState.Played)
                    .OrderByDescending(item => item.PlayState.TimesPlayed)
                    .Select(item => (Track)item)
                    .DedupeTracks()
                    .Take(playlistsize * 5)
                    .Shuffle()
                    .Take(playlistsize)
                    .Select(item => (baseEntity)item)
                    .ToList());
                return e;
            }
        }
    }
}
