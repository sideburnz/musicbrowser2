using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class IconProvider : IDataProvider
    {
        private const string Name = "IconProvider";

        private const int MinDaysBetweenHits = 7;
        private const int MaxDaysBetweenHits = 14;
        private const int RefreshPercentage = 25;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

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

            Statistics.Hit(Name + ".hit");

            string IBNPath = Path.Combine(Path.Combine(Util.Config.GetInstance().GetStringSetting("ImagesByName"), "musicgenre"), dto.Title);
            dto.ThumbImage = ImageProvider.Load(ImageProvider.LocateFanArt(IBNPath, ImageType.Thumb));

            IEnumerable<string> backPaths = ImageProvider.LocateBackdropList(IBNPath);
            List<Bitmap> backImages = new List<Bitmap>();
            foreach (string back in backPaths)
            {
                backImages.Add(ImageProvider.Load(back));
            }
            dto.BackImages = backImages;

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "genre");
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
