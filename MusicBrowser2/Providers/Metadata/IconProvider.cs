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

        public bool isStale(DateTime lastAccess)
        {
            // refresh weekly
            return (lastAccess.AddDays(7) < DateTime.Now);
        }


        public ProviderSpeed Speed
        {
            get { return ProviderSpeed.Fast; }
        }
    }
}
