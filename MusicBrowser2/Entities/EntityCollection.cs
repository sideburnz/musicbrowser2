using System.Collections.Generic;
using System;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<baseEntity>, IEnumerable<baseEntity>
    {
        public void AddRange(IEnumerable<FileSystemItem> items)
        {
            foreach (FileSystemItem item in items)
            {
                baseEntity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                baseEntity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public new void Add(baseEntity e)
        {
            if (e != null)
            {
                base.Add(e);
            }
        }

        public void Add(IEnumerable<baseEntity> entities)
        {
            base.AddRange(entities);
        }

        public new void Sort()
        {
            //foreach (baseEntity e in this) { e.CacheSortName(); }
            base.Sort(new EntityCollectionSorter());
            for (int i = 0; i < Count; this[i].Index = i++) ;
        }

        //public EntityCollection Filter(EntityKind kind, string key, string value)
        //{
        //    EntityCollection ret = new EntityCollection();

        //    switch (key.ToLower())
        //    {
        //        case "genre":
        //            ret.Add(this.Where(item => item.Kind == kind && item.Genre == value));
        //            break;
        //        case "year":
        //            ret.Add(this.Where(item => item.Kind == kind && item.ReleaseDate.ToString("yyyy") == value));
        //            break;
        //        case "":
        //            ret.Add(this.Where(item => item.Kind == kind));
        //            break;
        //    }
        //    return ret;
        //}

        //public EntityCollection Filter(EntityKind kind)
        //{
        //    EntityCollection ret = new EntityCollection();

        //    ret.Add(this
        //        .Where(item => item.Kind == kind)
        //    );

        //    return ret;
        //}

        //public EntityCollection Filter(EntityKind kind, string value)
        //{
        //    EntityCollection ret = new EntityCollection();

        //    if (kind == EntityKind.None || kind == EntityKind.Folder)
        //    {
        //        ret.Add(this
        //            .Where(item => item.Title.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
        //            .OrderBy(item => item.Title)
        //        );
        //    }
        //    else
        //    {
        //        ret.Add(this
        //            .Where(item => item.Kind == kind
        //                && item.Title.StartsWith(value, StringComparison.CurrentCultureIgnoreCase)
        //                )
        //            .OrderBy(item => item.Title)
        //        );
        //    }
        //    return ret;
        //}

        //public EntityCollection Group(EntityKind kind, string key)
        //{
        //    EntityCollection ret = new EntityCollection();

        //    switch (key.ToLower())
        //    {
        //        case "year":
        //            {
        //                ret.Add(this
        //                    .Where(item => item.Kind == kind && item.ReleaseDate > DateTime.Parse("01-JAN-1000"))
        //                    .GroupBy(item => item.ReleaseDate.ToString("yyyy"))
        //                    .Select(item =>
        //                    new Entity
        //                    {
        //                        Path = kind.ToString() + "s by " + key,
        //                        Title = item.Key,
        //                        Duration = item.Sum(subitem => subitem.Duration),
        //                        ArtistCount = item.Sum(subitem => subitem.ArtistCount),
        //                        AlbumCount = item.Sum(subitem => subitem.AlbumCount),
        //                        TrackCount = item.Sum(subitem => subitem.TrackCount),
        //                        Label = key.ToLower(),
        //                        Kind = EntityKind.Virtual
        //                    }
        //                        )
        //                );
        //                break;
        //            }
        //        case "genre":
        //            {
        //                ret.Add(this
        //                    .Where(item => item.Kind == kind && !String.IsNullOrEmpty(item.Genre))
        //                    .GroupBy(item => item.Genre)
        //                    .Select(item =>
        //                        new Entity
        //                        {
        //                            Path = kind.ToString() + "s by " + key,
        //                            Title = item.Key,
        //                            Duration = item.Sum(subitem => subitem.Duration),
        //                            ArtistCount = item.Sum(subitem => subitem.ArtistCount),
        //                            AlbumCount = item.Sum(subitem => subitem.AlbumCount),
        //                            TrackCount = item.Sum(subitem => subitem.TrackCount),
        //                            Label = key.ToLower(),
        //                            Kind = EntityKind.Virtual
        //                        }
        //                        )
        //                );
        //                break;
        //            }
        //    }
        //    return ret;
        //}
    }

    #region Folder Sorter

    sealed class EntityCollectionSorter : IComparer<baseEntity>
    {
        public int Compare(baseEntity x, baseEntity y)
        {
            bool xIsItem = x.GetType().IsSubclassOf(typeof(Item));
            bool yIsItem = y.GetType().IsSubclassOf(typeof(Item));

            //folders (artists and albums) have a higher priority than playlists and tracks
            if (!xIsItem && yIsItem) { return -1; }
            if (xIsItem && !yIsItem) { return 1; }

            return string.Compare(x.Title, y.Title, true);
        }
    }

    #endregion
}
