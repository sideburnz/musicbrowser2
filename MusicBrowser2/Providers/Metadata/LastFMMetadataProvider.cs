using System;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.WebServiceProviders;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class LastFMMetadataProvider : IDataProvider
    {
        private const string Name = "Last.fm";

        private const int MinDaysBetweenHits = 1;
        private const int MaxDaysBetweenHits = 7;
        private const int RefreshWindow = MaxDaysBetweenHits - MinDaysBetweenHits;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);
        
        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            Logging.Logger.Debug(Name + ": " + dto.Path);
            dto.Outcome = DataProviderOutcome.Success;

            Statistics.GetInstance().Hit(Name + ".hit");

            WebServiceProvider lfmProvider = new LastFMWebProvider();

            switch (dto.DataType)
            {
                case DataTypes.Album:
                    {
                        #region killer questions

                        if (string.IsNullOrEmpty(dto.AlbumName))
                        {
                            dto.Outcome = DataProviderOutcome.InvalidInput;
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Album name" };
                            return dto;
                        }

                        if (string.IsNullOrEmpty(dto.AlbumArtist))
                        {
                            dto.Outcome = DataProviderOutcome.InvalidInput;
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Artist name" };
                            return dto;
                        }

                        #endregion

                        AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
                        albumDTO.Album = dto.AlbumName;
                        albumDTO.MusicBrainzID = dto.MusicBrainzId;
                        albumDTO.Artist = dto.AlbumArtist;
                        albumDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        AlbumInfoService albumService = new AlbumInfoService();
                        albumService.SetProvider(lfmProvider);
                        albumService.Fetch(albumDTO);

                        // handle error back from the provider
                        if (albumDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + albumDTO.Error + ")" };
                            return dto;
                        }

                        dto.PlayCount = albumDTO.Plays;
                        dto.Listeners = albumDTO.Listeners;
                        dto.TotalPlays = albumDTO.TotalPlays;

                        dto.Title = albumDTO.Album;
                        dto.Summary = albumDTO.Summary;
                        dto.MusicBrainzId = albumDTO.MusicBrainzID;

                        dto.ReleaseDate = albumDTO.Release;

                        if (!dto.hasThumbImage && !string.IsNullOrEmpty(albumDTO.Image))
                        {
                            dto.ThumbImage = ImageProvider.Download(albumDTO.Image, ImageType.Thumb);
                        }

                        break;
                    }
                case DataTypes.Artist:
                    {
                        #region killer questions

                        if (string.IsNullOrEmpty(dto.ArtistName))
                        {
                            dto.Outcome = DataProviderOutcome.InvalidInput;
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Artist name" };
                            return dto;
                        }

                        #endregion

                        ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
                        artistDTO.Artist = dto.ArtistName;
                        artistDTO.MusicBrainzID = dto.MusicBrainzId;
                        artistDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        ArtistInfoService artistService = new ArtistInfoService();
                        artistService.SetProvider(lfmProvider);
                        artistService.Fetch(artistDTO);

                        // handle error back from the provider
                        if (artistDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + artistDTO.Error + ")" };
                            return dto;
                        }

                        dto.PlayCount = artistDTO.Plays;
                        dto.Listeners = artistDTO.Listeners;
                        dto.TotalPlays = artistDTO.TotalPlays;

                        dto.ArtistName = artistDTO.Artist;
                        dto.MusicBrainzId = artistDTO.MusicBrainzID;
                        dto.Summary = artistDTO.Summary;

                        if (!dto.hasThumbImage && !string.IsNullOrEmpty(artistDTO.Image))
                        {
                            dto.ThumbImage = ImageProvider.Download(artistDTO.Image, ImageType.Thumb);
                        }

                        break;
                    }
                case DataTypes.Song:
                    {
                        #region killer questions

                        if (string.IsNullOrEmpty(dto.ArtistName))
                        {
                            dto.Outcome = DataProviderOutcome.InvalidInput;
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Artist name" };
                            return dto;
                        }

                        #endregion

                        TrackInfoDTO trackDTO = new TrackInfoDTO();
                        trackDTO.Track = dto.TrackName;
                        trackDTO.Artist = dto.ArtistName;
                        trackDTO.MusicBrainzID = dto.MusicBrainzId;
                        trackDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));

                        TrackInfoService trackService = new TrackInfoService();
                        trackService.SetProvider(lfmProvider);
                        trackService.Fetch(trackDTO);

                        // handle error back from the provider
                        if (trackDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + trackDTO.Error + ")" };
                            return dto;
                        }

                        dto.PlayCount = trackDTO.Plays;
                        dto.Listeners = trackDTO.Listeners;
                        dto.TotalPlays = trackDTO.TotalPlays;

                        dto.TrackName = trackDTO.Track;
                        dto.Summary = trackDTO.Summary;
                        dto.MusicBrainzId = trackDTO.MusicBrainzID;
                        dto.Favorite = trackDTO.Loved;

                        break;
                    }
            }

            return dto;
        }

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

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "artist") || (type.ToLower() == "album") || (type.ToLower() == "song");
        }

        public bool isStale(DateTime lastAccess)
        {
            // refresh fortnightly
            return RandomlyRefreshData(lastAccess);
        }
    }
}
