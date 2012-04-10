using System;
using System.Collections.Generic;
using System.Linq;
using MusicBrowser.Models;

namespace MusicBrowser.Entities
{
    public static class Extensions
    {
        // so we can inherit values, we need someway of working out the object
        // heirarchy, this returns the types back to the baseEntity
        public static IEnumerable<string> Tree(this baseEntity e)
        {
            Type node = e.GetType();
            List<String> ret = new List<String>();

            while (node != typeof(BaseModel))
            {
                ret.Add(node.Name);
                node = node.BaseType;
            }
            return ret;
        }

        public static bool InheritsFrom<T>(this object e)
        {
            return e is T;
        }

        public static IEnumerable<Track> DedupeTracks(this IEnumerable<Track> list)
        {
            return list
                .GroupBy(item => item, new TrackComparer())
                .Select(item => item.First());
        }
    }
}