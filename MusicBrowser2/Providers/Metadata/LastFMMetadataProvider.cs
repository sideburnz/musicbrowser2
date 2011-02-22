using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.WebServices.Services.LastFM;

namespace MusicBrowser.Providers.Metadata
{
    class LastFMMetadataProvider : IMetadataProvider
    {
        private const string MARKER = "LAST.FM";

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            //killer questions
            if (!Util.Config.getInstance().getBooleanSetting("UseInternetProviders")) { return entity; }
            if (entity.Properties.ContainsKey(MARKER))
            {
                // only check for new info once a day
                if (DateTime.Parse(entity.Properties[MARKER]) > DateTime.Now.AddDays(-1)) { return entity; }
            }

            Logging.Logger.Verbose("LastFMMetadataProvider.Fetch", "start");

            WebServiceProvider LFMProvider = new LastFMWebProvider();

            switch (entity.Kind)
            {
                case EntityKind.Album:
                    {
                        AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
                        albumDTO.Album = entity.Title;
                        if (entity.Parent.Kind.Equals(EntityKind.Artist)) { albumDTO.Artist = entity.Parent.Title; }
                        albumDTO.MusicBrowserID = entity.MusicBrainzID;
                        albumDTO.Username = (Util.Config.getInstance().getSetting("LastFMUserName"));

                        AlbumInfoService albumService = new AlbumInfoService();
                        albumService.setProvider(LFMProvider);
                        albumService.Fetch(albumDTO);

                        entity.Properties.Add("lfm.playcount", albumDTO.Plays.ToString());
                        if (albumDTO.Release > DateTime.MinValue)
                        {
                            entity.Properties.Add("release", albumDTO.Release.ToString("yyyy"));
                        }
                        if (string.IsNullOrEmpty(entity.IconPath) && !string.IsNullOrEmpty(albumDTO.Image))
                        {
                            string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                            ImageProvider.Save(ImageProvider.Download(albumDTO.Image, ImageType.Thumb), tmpThumb);
                            entity.IconPath = tmpThumb;
                        }
                        entity.Title = albumDTO.Album;
                        entity.Summary = albumDTO.Summary;
                        entity.MusicBrainzID = albumDTO.MusicBrowserID;

                        break;
                    }
                case EntityKind.Artist:
                    {
                        ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
                        artistDTO.Artist = entity.Title;
                        artistDTO.MusicBrowserID = entity.MusicBrainzID;
                        artistDTO.Username = (Util.Config.getInstance().getSetting("LastFMUserName"));

                        ArtistInfoService artistService = new ArtistInfoService();
                        artistService.setProvider(LFMProvider);
                        artistService.Fetch(artistDTO);

                        entity.Properties.Add("lfm.playcount", artistDTO.Plays.ToString());
                        entity.Title = artistDTO.Artist;
                        entity.MusicBrainzID = artistDTO.MusicBrowserID;

                        if (string.IsNullOrEmpty(entity.IconPath) && !string.IsNullOrEmpty(artistDTO.Thumb))
                        {
                            string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                            ImageProvider.Save(ImageProvider.Download(artistDTO.Thumb, ImageType.Thumb), tmpThumb);
                            entity.IconPath = tmpThumb;
                        }

                        break;
                    }
                case EntityKind.Playlist:
                    {
                        break;
                    }
                case EntityKind.Song:
                    {
                        break;
                    }
            }

            entity.Properties.Add(MARKER, DateTime.Now.ToString("yyyy-MMM-dd"));
            entity.Dirty = true;
            return entity;
        }

        #endregion
    }
}
