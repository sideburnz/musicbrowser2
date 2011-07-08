using System;
using System.Linq;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.HTBackdrop;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Providers.Metadata
{
    public class HTBackdropMetadataProvider //: IMetadataProvider
    {
        private const string Marker = "HTBACK";

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            // killer questions
            if (!Util.Config.GetInstance().GetBooleanSetting("UseInternetProviders")) { return entity; }
            if (!entity.Kind.Equals(EntityKind.Artist)) { return entity; }
            if (!String.IsNullOrEmpty(entity.IconPath) && !String.IsNullOrEmpty(entity.BackgroundPath)) { return entity; }
            if (entity.Properties.ContainsKey(Marker))
            {
                // only check fro new images once every seven days
                if (DateTime.Parse(entity.Properties[Marker]) > DateTime.Now.AddDays(-14)) { return entity; }
            }

#if DEBUG
            Logging.Logger.Verbose("HTBackdropMetadataProvider.Fetch(" + entity.Path + ")", "start");
#endif
            // set up the web service classes
            ArtistImageServiceDTO dto = new ArtistImageServiceDTO();
            ArtistImageService service = new ArtistImageService();

            // set up the search criteria
            dto.ArtistName = entity.Title;
            dto.ArtistMusicBrainzID = entity.MusicBrainzID;
            dto.GetBackdrops = String.IsNullOrEmpty(entity.BackgroundPath);
            dto.GetThumbs = String.IsNullOrEmpty(entity.IconPath);

            // use the HTB provider to execute the web service
            WebServiceProvider webProvider = new HTBackdropWebProvider();
            service.SetProvider(webProvider);
            service.Fetch(dto);

            // handle the response
            if (dto.ThumbList.Count > 0) 
            { 
                string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                // randomize the list
                dto.ThumbList = dto.ThumbList.OrderBy(i => new Guid()).ToList();
                ImageProvider.Save(ImageProvider.Download(dto.ThumbList[0], ImageType.Thumb), tmpThumb);
                entity.IconPath = tmpThumb;
            }
            if (dto.BackdropList.Count > 0) 
            { 
                string tmpBack = Util.Helper.ImageCacheFullName(entity.CacheKey, "Backgrounds");
                dto.BackdropList = dto.BackdropList.OrderBy(i => new Guid()).ToList();
                ImageProvider.Save(ImageProvider.Download(dto.BackdropList[0], ImageType.Backdrop), tmpBack);
                entity.BackgroundPath = tmpBack;
            }

            entity.SetProperty(Marker, DateTime.Now.ToString("yyyy-MMM-dd"), true);

            entity.Dirty = true;
            entity.CalculateValues();
            return entity;
        }

        #endregion
    }
}
