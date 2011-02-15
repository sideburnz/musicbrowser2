using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.WebServices.Services.HTBackdrop;

namespace MusicBrowser.Providers.Metadata
{
    class HTBackdropMetadataProvider : IMetadataProvider
    {
        private const string MARKER = "HTBACK";

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            // killer questions
            if (!entity.Kind.Equals(EntityKind.Artist)) { return entity; }
            if (!String.IsNullOrEmpty(entity.IconPath) && !String.IsNullOrEmpty(entity.BackgroundPath)) { return entity; }
            if (entity.Properties.ContainsKey(MARKER))
            {
                // only check fro new images once every seven days
                if (DateTime.Parse(entity.Properties[MARKER]) > DateTime.Now.AddDays(-7)) { return entity; }
            }

            Logging.Logger.Verbose("HTBackdropMetadataProvider.Fetch(" + entity.Path + ")", "start");

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
            service.setProvider(webProvider);
            service.Fetch(dto);

            // handle the response
            if (dto.ThumbList.Count > 0) 
            { 
                string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                ImageProvider.Save(ImageProvider.Download(dto.ThumbList[0], ImageType.Thumb), tmpThumb);
                entity.IconPath = tmpThumb;
            }
            if (dto.BackdropList.Count > 0) 
            { 
                string tmpBack = Util.Helper.ImageCacheFullName(entity.CacheKey, "Backgrounds");
                ImageProvider.Save(ImageProvider.Download(dto.BackdropList[0], ImageType.Backdrop), tmpBack);
                entity.BackgroundPath = tmpBack;
            }

            entity.Properties.Add(MARKER, DateTime.Now.ToString("yyyy-MMM-dd"));

            entity.Dirty = true;
            return entity;
        }

        #endregion
    }
}
