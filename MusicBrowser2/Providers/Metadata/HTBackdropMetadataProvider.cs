using System;
using System.Drawing;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.HTBackdrop;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    public class HTBackdropMetadataProvider : IDataProvider
    {
        private const string Name = "HTBackdrops";

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

            if (!Util.Config.GetInstance().GetBooleanSetting("UseInternetProviders"))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { "Internet Providers Disabled" };
                return dto;
            }

            if (!Util.Config.GetInstance().GetBooleanSetting("EnableFanArt"))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { "Fan Art Disabled" };
                return dto;
            }

            if (dto.hasBackImage && dto.hasThumbImage)
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { "Entity already has images" };
                return dto;
            }

            if (string.IsNullOrEmpty(dto.ArtistName))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { Name + ": no artist specificed [" + dto.Path + "]" };
                return dto;
            }

            #endregion

            Statistics.Hit(Name + ".hit");

            // set up the web service classes
            ArtistImageServiceDTO serviceDTO = new ArtistImageServiceDTO();
            ArtistImageService service = new ArtistImageService();

            // set up the search criteria
            serviceDTO.ArtistName = dto.ArtistName;
            serviceDTO.ArtistMusicBrainzID = dto.MusicBrainzId;
            serviceDTO.GetBackdrops = !dto.hasBackImage;
            serviceDTO.GetThumbs = !dto.hasThumbImage;

            // use the HTTP provider to execute the web service
            WebServiceProvider webProvider = new HTBackdropWebProvider();
            service.SetProvider(webProvider);
            service.Fetch(serviceDTO);

            // handle error back from the provider
            if (serviceDTO.Status == WebServiceStatus.Error)
            {
                dto.Outcome = DataProviderOutcome.SystemError;
                dto.Errors = new System.Collections.Generic.List<string> { "SystemError Web Service Error (" + serviceDTO.Error + ") " + serviceDTO.ArtistName };
                return dto;
            }

            // handle the response
            if (serviceDTO.ThumbList.Count > 0)
            {
                // get random item
                dto.ThumbImage = ImageProvider.Download(serviceDTO.ThumbList[Rnd.Next(serviceDTO.ThumbList.Count)], ImageType.Thumb);
            }

            if (serviceDTO.BackdropList.Count > 0)
            {
                // limit to 10 backdrops
                foreach (string img in serviceDTO.BackdropList.Take(10))
                {
                    // wrap in a try, if one fails we don't want them all to fail
                    try
                    {
                        Bitmap i = ImageProvider.Download(img, ImageType.Backdrop);
                        if (i != null) 
                        { 
                            dto.BackImages.Add(i); 
                        }
                    }
                    catch { }
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
            return (type.ToLower() == "artist");
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
            get { return ProviderType.Peripheral; }
        }
    }
}
