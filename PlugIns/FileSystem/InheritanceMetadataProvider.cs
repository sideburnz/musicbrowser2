using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Metadata
{
    class InheritanceMetadataProvider : baseMetadataProvider
    {
        /*
         * - album artist on tracks populates album
         * - year on tracks populates album
         */

        public InheritanceMetadataProvider()
        {
            Name = "InheritanceMetadataProvider";
            MinDaysBetweenHits = 1;
            MaxDaysBetweenHits = 10;
            RefreshPercentage = 25;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return dto.InheritsFrom<Album>() || dto.InheritsFrom<Season>();
        }

        protected override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Directory.Exists(dto.Path)) { return false; }
            return true;
        }

        protected override ProviderOutcome DoWork(baseEntity dto)
        {
            if (dto.InheritsFrom<Album>())
            {
                if (DoWorkAlbum((Album)dto))
                {
                    return ProviderOutcome.Success;
                }
            }
            if (dto.InheritsFrom<Season>())
            {
                if (DoWorkSeason((Season)dto))
                {
                    return ProviderOutcome.Success;
                }
            }
            return ProviderOutcome.InvalidInput;
        }

        private bool DoWorkAlbum(Album dto)
        {
            Dictionary<string, int> artists = new Dictionary<string, int>();
            Dictionary<string, int> albumartists = new Dictionary<string, int>();
            DateTime latestrelease = DateTime.MinValue;

            try
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(dto.Path).FilterInternalFiles();
                foreach (FileSystemItem item in items)
                {
                    if (Helper.GetKnownType(item) == Helper.KnownType.Track)
                    {
                        Track t = (Track)Factory.GetItem(item);
                        if (!String.IsNullOrEmpty(t.AlbumArtist))
                        {
                            if (albumartists.ContainsKey(t.AlbumArtist))
                            {
                                albumartists[t.AlbumArtist]++;
                            }
                            else
                            {
                                albumartists.Add(t.AlbumArtist, 1);
                            }
                        }
                        if (!String.IsNullOrEmpty(t.Artist))
                        {
                            if (artists.ContainsKey(t.Artist))
                            {
                                artists[t.Artist]++;
                            }
                            else
                            {
                                artists.Add(t.Artist, 1);
                            }
                        }
                        if (t.ReleaseDate > latestrelease)
                        {
                            latestrelease = t.ReleaseDate;
                        }
                    }
                }

                if (dto.ReleaseDate.Year <= 1000 && latestrelease.Year > 1000)
                {
                    dto.ReleaseDate = latestrelease;
                }
                if (String.IsNullOrEmpty(dto.AlbumArtist))
                {
                    if (albumartists.Count == 0)
                    {
                        if (artists.Count == 0)
                        {
                            return true;
                        }
                        dto.AlbumArtist = artists.OrderByDescending(item => item.Value).FirstOrDefault().Key;
                        return true;
                    }
                    dto.AlbumArtist = albumartists.OrderByDescending(item => item.Value).FirstOrDefault().Key;
                }

                return true;
            }
            catch (Exception e)
            {
                Logging.LoggerEngineFactory.Error(e);
            }

            return false;
        }

        // - show name populates seasons
        private bool DoWorkSeason(Season dto)
        {
            if (!String.IsNullOrEmpty(dto.Show))
            {
                return true;
            }

            try
            {
                Dictionary<string, int> stats = new Dictionary<string, int>();
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(dto.Path).FilterInternalFiles();

                foreach (FileSystemItem item in items)
                {
                    if (Helper.IsEpisode(item.Name))
                    {
                        Episode e = (Episode)Factory.GetItem(item);
                        if (!String.IsNullOrEmpty(e.ShowName))
                        {
                            if (stats.ContainsKey(e.ShowName))
                            {
                                stats[e.ShowName]++;
                            }
                            else
                            {
                                stats.Add(e.ShowName, 1);
                            }
                        }
                    }
                }

                if (stats.Count == 0)
                {
                    return true;
                }

                dto.Show = stats.OrderByDescending(item => item.Value).FirstOrDefault().Key;

                return true;
            }
            catch (Exception e)
            {
                Logging.LoggerEngineFactory.Error(e);
            }

            return false;
        }

    }
}
