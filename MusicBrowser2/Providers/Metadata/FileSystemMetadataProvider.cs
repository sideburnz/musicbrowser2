using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    // populates children and all descendants

    class FileSystemMetadataProvider : IDataProvider
    {
        private const string Name = "FileSystem";

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose(Name + ": " + dto.Path, "start");
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

            Statistics.GetInstance().Hit(Name + ".hit");

            int duration = 0;
            int albums = 0;
            int artists = 0;
            int tracks = 0;

            EntityFactory factory = new EntityFactory();

            IEnumerable<FileSystemItem> allPaths = FileSystemProvider.GetAllSubPaths(dto.Path);
            foreach (FileSystemItem item in allPaths)
            {
                IEntity e = factory.GetItem(item);

                if (e != null)
                {
                    switch (e.Kind)
                    {
                        case EntityKind.Song:
                            {
                                tracks++;
                                duration += e.Duration;
                                break;
                            }
                        case EntityKind.Album:
                            {
                                albums++;
                                break;
                            }
                        case EntityKind.Artist:
                            {
                                artists++;
                                break;
                            }
                    }
                }
            }

            dto.TrackCount = tracks;
            dto.Duration = duration;
            dto.ArtistCount = artists;
            dto.AlbumCount = albums;

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

        public bool isStale(DateTime lastAccess)
        {
            // refresh weekly
            return (lastAccess.AddDays(7) < DateTime.Now);
        }
    }
}
