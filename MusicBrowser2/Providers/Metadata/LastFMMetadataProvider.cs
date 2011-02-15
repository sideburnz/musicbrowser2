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
    //lastfm_playcount
    //lastfm_loved

    class LastFMMetadataProvider : IMetadataProvider
    {
        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            //killer questions
            if (!Util.Config.getInstance().getBooleanSetting("UseInternetProviders")) { return entity; }

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

                        entity.Title = albumDTO.Album;
                        entity.Summary = albumDTO.Summary;
                        entity.MusicBrainzID = albumDTO.MusicBrowserID;
                        break;
                    }
                case EntityKind.Artist:
                    {
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
            return entity;
        }

        #endregion
    }
}
