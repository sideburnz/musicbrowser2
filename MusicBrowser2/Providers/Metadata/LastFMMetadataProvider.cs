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
                // only check for new info once a week
                if (DateTime.Parse(entity.Properties[MARKER]) > DateTime.Now.AddDays(-7)) { return entity; }
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

                        UpdateDic(entity.Properties, "lfm.playcount", albumDTO.Plays.ToString());
                        if (albumDTO.Release > DateTime.MinValue)
                        {
                            if (!entity.Properties.ContainsKey("release"))
                            {
                                UpdateDic(entity.Properties, "release", albumDTO.Release.ToString("yyyy"));
                            }
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

                        UpdateDic(entity.Properties, "lfm.playcount", artistDTO.Plays.ToString());
                        entity.Title = artistDTO.Artist;
                        entity.MusicBrainzID = artistDTO.MusicBrowserID;
                        entity.Summary = artistDTO.Summary;

                        if (string.IsNullOrEmpty(entity.IconPath) && !string.IsNullOrEmpty(artistDTO.Image))
                        {
                            string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                            ImageProvider.Save(ImageProvider.Download(artistDTO.Image, ImageType.Thumb), tmpThumb);
                            entity.IconPath = tmpThumb;
                        }

                        break;
                    }
                case EntityKind.Playlist:
                    {
                        return entity;
                    }
                case EntityKind.Song:
                    {
                        break;
                    }
            }

            if (entity.Properties.ContainsKey(MARKER))
            {
                entity.Properties[MARKER] = DateTime.Now.ToString("yyyy-MMM-dd");
            }
            else
            {
                entity.Properties.Add(MARKER, DateTime.Now.ToString("yyyy-MMM-dd"));
            }
            entity.Dirty = true;
            return entity;
        }

        #endregion

        public IDictionary<string, string> UpdateDic(IDictionary<string, string> dic, string key, string val)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = val;
            }
            else
            {
                dic.Add(key, val);
            }
            return dic;
        }
    }
}
