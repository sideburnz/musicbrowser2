using System;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Providers.Metadata
{
    class LastFMMetadataProvider : IMetadataProvider
    {
        private const string Marker = "LAST.FM";


        private const int MinDaysBetweenHits = 1;
        private const int MaxDaysBetweenHits = 7;
        private const int RefreshWindow = MaxDaysBetweenHits - MinDaysBetweenHits;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);
        
        /// <summary>
        /// This determines if data should be refreshed, it makes it more likely
        /// the older the data is to be refreshed.
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        private static bool RandomlyRefreshData(DateTime stamp)
        {
            int dataAge = (DateTime.Today.Subtract(stamp)).Days;
            int refreshProbability = (10 * (dataAge / (RefreshWindow - MinDaysBetweenHits))) ^ 2;
            if (dataAge <= MinDaysBetweenHits) { return false; }
            return (refreshProbability >= Rnd.Next(100));
        }

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            //killer questions
            if (!Util.Config.GetInstance().GetBooleanSetting("UseInternetProviders")) { return entity; }
            if (entity.Properties.ContainsKey(Marker))
            {
                if (!RandomlyRefreshData(DateTime.Parse(entity.Properties[Marker]))) { return entity; } 
            }
#if DEBUG
            Logging.LoggerFactory.Verbose("LastFMMetadataProvider.Fetch", "start");
#endif
            Statistics.GetInstance().Hit("lastfm.hit");

            WebServiceProvider lfmProvider = new LastFMWebProvider();

            switch (entity.Kind)
            {
                case EntityKind.Album:
                    {
                        AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
                        albumDTO.Album = entity.Title;
                        if (entity.Parent.Kind.Equals(EntityKind.Artist)) { albumDTO.Artist = entity.Parent.Title; }
                        albumDTO.MusicBrainzID = entity.MusicBrainzID;
                        albumDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        AlbumInfoService albumService = new AlbumInfoService();
                        albumService.SetProvider(lfmProvider);
                        albumService.Fetch(albumDTO);
                        if (albumDTO.Status != WebServiceStatus.Success) { break; }

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
                        artistDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        ArtistInfoService artistService = new ArtistInfoService();
                        artistService.SetProvider(lfmProvider);
                        artistService.Fetch(artistDTO);

                        if (artistDTO.Status != WebServiceStatus.Success) { break; }

                        entity.SetProperty("lfm.playcount", artistDTO.Plays.ToString(), true);
                        entity.SetProperty("lfm.listeners", artistDTO.Listeners.ToString(), true);
                        entity.SetProperty("lfm.totalplays", artistDTO.TotalPlays.ToString(), true);

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
                        trackDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        TrackInfoService trackService = new TrackInfoService();
                        trackService.SetProvider(lfmProvider);
                        trackService.Fetch(trackDTO);
                        if (trackDTO.Status != WebServiceStatus.Success) { break; }

                        entity.SetProperty("lfm.playcount", trackDTO.Plays.ToString(), true);
                        entity.SetProperty("lfm.listeners", trackDTO.Listeners.ToString(), true);
                        entity.SetProperty("lfm.totalplays", trackDTO.TotalPlays.ToString(), true);

                        entity.Title = trackDTO.Track;
                        entity.Summary = trackDTO.Summary;
                        entity.MusicBrainzID = trackDTO.MusicBrainzID;
                        entity.SetProperty("lfm.loved", trackDTO.Loved.ToString(), true);

                        break;
                    }
            }

            entity.SetProperty(Marker, DateTime.Now.ToString("yyyy-MMM-dd"), true);

            entity.Dirty = true;
            entity.CalculateValues();
            return entity;
        }

        #endregion
    }
}
