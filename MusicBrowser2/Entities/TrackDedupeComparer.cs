using System.Collections.Generic;

namespace MusicBrowser.Entities
{
    sealed class TrackDedupeComparer : IEqualityComparer<Track>
    {
        static string Key(Track a)
        {
            return (a.Artist + ":" + a.Title).ToLower();
        }

        public bool Equals(Track x, Track y)
        {
            return (Key(x) == Key(y));
        }

        public int GetHashCode(Track obj)
        {
            return Key(obj).GetHashCode();
        }
    }
}