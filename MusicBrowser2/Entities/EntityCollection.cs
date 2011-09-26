﻿using System.Collections.Generic;
using System;
using System.Linq;
using MusicBrowser.CacheEngine;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    public class EntityCollection : List<Entity>, IEnumerable<Entity>
    {
        public void Populate(IEnumerable<FileSystemItem> items)
        {
            foreach (FileSystemItem item in items)
            {
                Entity entity = EntityFactory.GetItem(item);
                Add(entity);
            }
        }

        public void IndexItems()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Index = i;
            }
        }

        public new void Add(Entity e)
        {
            if (e != null)
            {
                e.UpdateValues();
                base.Add(e);
            }
        }

        public new void Add(IEnumerable<Entity> entities)
        {
            base.AddRange(entities);
        }

        public new void Sort()
        {
            base.Sort(new EntityCollectionSorter());
        }

        public EntityCollection Filter(EntityKind kind, string key, string value)
        {
            EntityCollection ret = new EntityCollection();

            switch (key.ToLower())
            {
                case "genre":
                    ret.Add(this.Where(item => item.Kind == kind && item.Genre == value));
                    break;
                case "year":
                    ret.Add(this.Where(item => item.Kind == kind && item.ReleaseDate.ToString("yyyy") == value));
                    break;
            }
            return ret;
        }

        public EntityCollection Group(EntityKind kind, string key)
        {
            EntityCollection ret = new EntityCollection();

            switch (key.ToLower())
            {
                case "year":
                    {
                        ret.Add(this
                            .Where(item => item.Kind == kind && item.ReleaseDate > DateTime.Parse("01-JAN-1000"))
                            .GroupBy(item => item.ReleaseDate.ToString("yyyy"))
                            .Select(item =>
                            new Entity
                                {
                                    Path = kind.ToString().ToLower() + "s by " + key.ToLower(),
                                    Title = item.Key,
                                    Duration = item.Sum(subitem => subitem.Duration),
                                    ArtistCount = item.Sum(subitem => subitem.ArtistCount),
                                    AlbumCount = item.Sum(subitem => subitem.AlbumCount),
                                    TrackCount = item.Sum(subitem => subitem.TrackCount),
                                    Label = key.ToLower(),
                                    Kind = EntityKind.Virtual
                                }
                                )
                        );
                        break;
                    }
                case "genre":
                    {
                        ret.Add(this
                            .Where(item => item.Kind == kind && !String.IsNullOrEmpty(item.Genre))
                            .GroupBy(item => item.Genre)
                            .Select(item =>
                                new Entity
                                {
                                    Path = kind.ToString().ToLower() + "s by " + key.ToLower(),
                                    Title = item.Key,
                                    Duration = item.Sum(subitem => subitem.Duration),
                                    ArtistCount = item.Sum(subitem => subitem.ArtistCount),
                                    AlbumCount = item.Sum(subitem => subitem.AlbumCount),
                                    TrackCount = item.Sum(subitem => subitem.TrackCount),
                                    Label = key.ToLower(),
                                    Kind = EntityKind.Virtual
                                }
                                )
                        );
                        break;
                    }
            }
            return ret;
        }
    }

    #region Folder Sorter

    sealed class EntityCollectionSorter : IComparer<Entity>
    {
        public int Compare(Entity x, Entity y)
        {
            bool xIsItem = x.Playable;
            bool yIsItem = y.Playable;

            // folders (artists and albums) have a higher priority than playlists and songs
            if (!xIsItem && yIsItem) { return -1; }
            if (xIsItem && !yIsItem) { return 1; }

            return string.Compare(x.SortName, y.SortName, true);
        }
    }

    #endregion
}
