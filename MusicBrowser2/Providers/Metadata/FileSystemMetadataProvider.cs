using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    // populates children and all descendants

    class FileSystemMetadataProvider : IDataProvider
    {
        private const string Name = "FileSystemMetadataProvider";

        private const int MinDaysBetweenHits = 5;
        private const int MaxDaysBetweenHits = 10;
        private const int RefreshPercentage = 50;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif

            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions
            if (!Directory.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }
            #endregion

            Statistics.Hit(Name + ".hit");

            int duration = 0;
            int albums = 0;
            int artists = 0;
            int tracks = 0;
            List<string> children = new List<string>();

            IEnumerable<FileSystemItem> allPaths = FileSystemProvider.GetAllSubPaths(dto.Path);
            foreach (FileSystemItem item in allPaths)
            {
                Entity e = EntityFactory.GetItem(item);

                if (e != null)
                {
                    switch (e.Kind)
                    {
                        case EntityKind.Track:
                            {
                                tracks++;
                                duration += e.Duration;

                                if (dto.DataType == DataTypes.Album) { children.Add(e.Title); }
                                break;
                            }
                        case EntityKind.Album:
                            {
                                albums++;

                                if (dto.DataType == DataTypes.Artist) { children.Add(e.Title); }
                                break;
                            }
                        case EntityKind.Artist:
                            {
                                artists++;

                                if (dto.DataType == DataTypes.Genre) { children.Add(e.Title); }
                                break;
                            }
                    }
                }
            }

            dto.TrackCount = tracks;
            dto.Duration = duration;
            dto.ArtistCount = artists;
            dto.AlbumCount = albums;

            if (String.IsNullOrEmpty(dto.Summary))
            {
                children.Sort();
                StringBuilder sb = new StringBuilder();
                foreach (string child in children)
                {
                    sb.Append(child + "\n");
                }
                dto.Summary = sb.ToString();
            }

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return ((type.ToLower() == "album") || (type.ToLower() == "artist") || (type.ToLower() == "genre"));
        }

        /// <summary>
        /// refresh requests between the min and max refresh period have 10% chance of refreshing
        /// </summary>
        private static bool RandomlyRefreshData(DateTime stamp)
        {
            // if it's never refreshed, refresh it
            if (stamp < DateTime.Parse("01-JAN-1000")) { return true; }

            // if it's less then the min, don't refresh if it's older than the max then do refresh
            int dataAge = (DateTime.Today.Subtract(stamp)).Days;
            if (dataAge <= MinDaysBetweenHits) { return false; }
            if (dataAge >= MaxDaysBetweenHits) { return true; }

            // otherwise refresh randomly
            return (Rnd.Next(100) >= RefreshPercentage);
        }

        public bool isStale(DateTime lastAccess)
        {
            return RandomlyRefreshData(lastAccess);
        }

        public ProviderType Type
        {
            get { return ProviderType.Core; }
        }
    }
}
