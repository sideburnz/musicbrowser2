using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.WebServices.Services.LastFM;

namespace MusicBrowser.Providers.Metadata
{
    class LastFMMetadataProvider : IMetadataProvider
    {
        private const string MARKER = "LAST.FM";


        private const int MIN_DAYS_BETWEEN_HITS = 1;
        private const int MAX_DAYS_BETWEEN_HITS = 7;
        private const int REFRESH_WINDOW = MAX_DAYS_BETWEEN_HITS - MIN_DAYS_BETWEEN_HITS;

        private static Random Rnd = new Random(DateTime.Now.Millisecond);
        
        /// <summary>
        /// This determines if data should be refreshed, it makes it more likely
        /// the older the data is to be refreshed.
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        private static bool RandomlyRefreshData(DateTime stamp)
        {
            int DataAge = (DateTime.Today.Subtract(stamp)).Days;
            int RefreshProbability = (10 * (DataAge / (REFRESH_WINDOW - MIN_DAYS_BETWEEN_HITS))) ^ 2;
            if (DataAge <= MIN_DAYS_BETWEEN_HITS) { return false; }
            return (RefreshProbability >= Rnd.Next(100));
        }

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            //killer questions
            if (!Util.Config.getInstance().getBooleanSetting("UseInternetProviders")) { return entity; }
            if (entity.Properties.ContainsKey(MARKER))
            {
                if (!RandomlyRefreshData(DateTime.Parse(entity.Properties[MARKER]))) { return entity; } 
            }
#if DEBUG
            Logging.Logger.Verbose("LastFMMetadataProvider.Fetch", "start");
#endif
            MusicBrowser.Providers.Statistics.GetInstance().Hit("lastfm.hit");

            WebServiceProvider LFMProvider = new LastFMWebProvider();

            switch (entity.Kind)
            {
                case EntityKind.Album:
                    {
                        AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
                        albumDTO.Album = entity.Title;
                        if (entity.Parent.Kind.Equals(EntityKind.Artist)) { albumDTO.Artist = entity.Parent.Title; }
                        albumDTO.MusicBrainzID = entity.MusicBrainzID;
                        albumDTO.Username = (Util.Config.getInstance().getSetting("LastFMUserName"));

                        AlbumInfoService albumService = new AlbumInfoService();
                        albumService.setProvider(LFMProvider);
                        albumService.Fetch(albumDTO);

                        entity.SetProperty("lfm.playcount", albumDTO.Plays.ToString(), true);
                        entity.SetProperty("lfm.listeners", albumDTO.Listeners.ToString(), true);
                        entity.SetProperty("lfm.totalplays", albumDTO.TotalPlays.ToString(), true);

                        if (albumDTO.Release > DateTime.MinValue)
                        {
                            if (!entity.Properties.ContainsKey("release"))
                            {
                                entity.SetProperty("release", albumDTO.Release.ToString("yyyy"), true);
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
                        entity.MusicBrainzID = albumDTO.MusicBrainzID;

                        break;
                    }
                case EntityKind.Artist:
                    {
                        ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
                        artistDTO.Artist = entity.Title;
                        artistDTO.MusicBrainzID = entity.MusicBrainzID;
                        artistDTO.Username = (Util.Config.getInstance().getSetting("LastFMUserName"));

                        ArtistInfoService artistService = new ArtistInfoService();
                        artistService.setProvider(LFMProvider);
                        artistService.Fetch(artistDTO);

                        if (artistDTO.Plays > 0)
                        {
                            entity.SetProperty("lfm.playcount", artistDTO.Plays.ToString(), true);
                            entity.SetProperty("lfm.listeners", artistDTO.Listeners.ToString(), true);
                            entity.SetProperty("lfm.totalplays", artistDTO.TotalPlays.ToString(), true);
                        } 
                        else
                        {
                            entity.SetProperty("lfm.playcount", string.Empty, true);
                            entity.SetProperty("lfm.listeners", string.Empty, true);
                            entity.SetProperty("lfm.totalplays", string.Empty, true);
                        }

                        entity.Title = artistDTO.Artist;
                        entity.MusicBrainzID = artistDTO.MusicBrainzID;
                        entity.Summary = artistDTO.Summary;

                        if (string.IsNullOrEmpty(entity.IconPath) && !string.IsNullOrEmpty(artistDTO.Image))
                        {
                            string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                            ImageProvider.Save(ImageProvider.Download(artistDTO.Image, ImageType.Thumb), tmpThumb);
                            entity.IconPath = tmpThumb;
                        }

                        break;
                    }
                case EntityKind.Song:
                    {
                        TrackInfoDTO trackDTO = new TrackInfoDTO();
                        trackDTO.Track = entity.Title;
                        if (entity.Properties.ContainsKey("artist")) { trackDTO.Artist = entity.Properties["artist"]; }
                        trackDTO.MusicBrainzID = entity.MusicBrainzID;
                        trackDTO.Username = (Util.Config.getInstance().getSetting("LastFMUserName"));

                        TrackInfoService trackService = new TrackInfoService();
                        trackService.setProvider(LFMProvider);
                        trackService.Fetch(trackDTO);

                        if (trackDTO.Plays > 0)
                        {
                            entity.SetProperty("lfm.playcount", trackDTO.Plays.ToString(), true);
                            entity.SetProperty("lfm.listeners", trackDTO.Listeners.ToString(), true);
                            entity.SetProperty("lfm.totalplays", trackDTO.TotalPlays.ToString(), true);
                        }
                        else
                        {
                            entity.SetProperty("lfm.playcount", string.Empty, true);
                            entity.SetProperty("lfm.listeners", string.Empty, true);
                            entity.SetProperty("lfm.totalplays", string.Empty, true);
                        }

                        entity.Title = trackDTO.Track;
                        entity.Summary = trackDTO.Summary;
                        entity.MusicBrainzID = trackDTO.MusicBrainzID;
                        entity.SetProperty("lfm.loved", trackDTO.Loved.ToString(), true);

                        break;
                    }
            }

            entity.SetProperty(MARKER, DateTime.Now.ToString("yyyy-MMM-dd"), true);

            entity.Dirty = true;
            entity.CalculateValues();
            return entity;
        }

        #endregion
    }
}
