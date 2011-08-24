using System;
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

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            Logging.Logger.Debug(Name + ": " + dto.Path);
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            if (dto.hasBackImage && dto.hasThumbImage)
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { "Entity already has images" };
                return dto;
            }

            if (string.IsNullOrEmpty(dto.ArtistName))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new System.Collections.Generic.List<string> { "Unknown Artist" };
                return dto;
            }

            #endregion

            Statistics.GetInstance().Hit(Name + ".hit");

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
                dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + serviceDTO.Error + ")" };
                return dto;
            }

            // handle the response
            if (serviceDTO.ThumbList.Count > 0)
            {
                // randomize the list
                serviceDTO.ThumbList = serviceDTO.ThumbList.OrderBy(i => new Guid()).ToList();
                dto.ThumbImage = ImageProvider.Download(serviceDTO.ThumbList[0], ImageType.Thumb);
            }
            if (serviceDTO.BackdropList.Count > 0)
            {
                // randomize the list
                serviceDTO.ThumbList = serviceDTO.BackdropList.OrderBy(i => new Guid()).ToList();
                dto.BackImage = ImageProvider.Download(serviceDTO.BackdropList[0], ImageType.Backdrop);
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

        public bool isStale(DateTime lastAccess)
        {
            // refresh fortnightly
            return (lastAccess.AddDays(14) < DateTime.Now);
        }

    }
}
