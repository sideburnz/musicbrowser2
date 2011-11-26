using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using System.Text.RegularExpressions;

namespace MusicBrowser.Providers.Metadata
{
    // episode and season logic pulled from MediaBrowser

    class VideoFilenameMetadataProvider : IDataProvider
    {
        private const string Name = "VideoFilenameMetadataProvider";

        private const int MinDaysBetweenHits = 7;
        private const int MaxDaysBetweenHits = 14;
        private const int RefreshPercentage = 25;

        private static readonly Regex[] episodeExpressions = new Regex[] {
                        //new Regex(@"^[s|S]?(?<seasonnumber>\d{1,2})[x|X](?<epnumber>\d{1,3})\W*(?<epname>[\w\s]*)"),   // 01x02 blah.avi S01x01 balh.avi
                        new Regex(@"^[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<epnumber>\d{1,3})\W*(?<epname>.*)") // S01E02 blah.avi, S01xE01 blah.avi
        };

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            #endregion

            Statistics.Hit(Name + ".hit");

            foreach (Regex r in episodeExpressions)
            {
                Match m = r.Match(System.IO.Path.GetFileNameWithoutExtension(dto.Path));
                if (m.Success)
                {
                    int i = 0;
                    if (int.TryParse(m.Groups["epnumber"].Value, out i))
                    {
                        dto.Episode = i;
                    }
                    if (int.TryParse(m.Groups["seasonnumber"].Value, out i))
                    {
                        dto.Season = i;
                    }
                    try
                    {
                        dto.Title = m.Groups["epname"].Value;
                    }
                    catch
                    {
                        dto.Title = System.IO.Path.GetFileNameWithoutExtension(dto.Path);
                    }
                }
            }

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "episode");
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

            // otherwise refresh randomly (95% don't refresh each run)
            return (Rnd.Next(100) >= RefreshPercentage);
        }

        public bool isStale(DateTime lastAccess)
        {
            return RandomlyRefreshData(lastAccess);
        }

        public ProviderType Type
        {
            get { return ProviderType.Peripheral; }
        }

    }
}
