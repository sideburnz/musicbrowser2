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

        private const int MinDaysBetweenHits = 0;
        private const int MaxDaysBetweenHits = 100;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);
        
        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose(Name + ": " + dto.Path, "start");
#endif
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
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing album data: Album name [" + dto.Path + "]" };
                            return dto;
                        }

                        if (string.IsNullOrEmpty(dto.AlbumArtist))
                        {
                            dto.Outcome = DataProviderOutcome.InvalidInput;
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing data: Artist name [" + dto.Path + "]" };
                            return dto;
                        }

                        #endregion

                        AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
                        albumDTO.Album = dto.AlbumName;
                        albumDTO.MusicBrainzID = dto.MusicBrainzId;
                        albumDTO.Artist = dto.AlbumArtist;
                        albumDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));
                        if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
                        {
                            albumDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
                        }
                        else { albumDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

                        AlbumInfoService albumService = new AlbumInfoService();
                        albumService.SetProvider(lfmProvider);
                        albumService.Fetch(albumDTO);

                        // handle error back from the provider
                        if (albumDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + albumDTO.Error + ") Artist (" + albumDTO.Artist + ") Album (" + albumDTO.Album + ")" };
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
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing artist data: Artist name [" + dto.Path + "]" };
                            return dto;
                        }

                        #endregion

                        ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
                        artistDTO.Artist = dto.ArtistName;
                        artistDTO.MusicBrainzID = dto.MusicBrainzId;
                        artistDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));
                        if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
                        {
                            artistDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
                        }
                        else { artistDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

                        ArtistInfoService artistService = new ArtistInfoService();
                        artistService.SetProvider(lfmProvider);
                        artistService.Fetch(artistDTO);

                        // handle error back from the provider
                        if (artistDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + artistDTO.Error + ") Artist (" + artistDTO.Artist + ")" };
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
                            dto.Errors = new System.Collections.Generic.List<string> { "Missing song data: Artist name [" + dto.Path + "]" };
                            return dto;
                        }
                        
                        #endregion

                        TrackInfoDTO trackDTO = new TrackInfoDTO();
                        trackDTO.Track = dto.TrackName;
                        trackDTO.Artist = dto.ArtistName;
                        trackDTO.MusicBrainzID = dto.MusicBrainzId;
                        trackDTO.Username = (Util.Config.GetInstance().GetSetting("LastFMUserName"));
                        if (dto.ProviderTimeStamps.ContainsKey(FriendlyName()))
                        {
                            trackDTO.lastAccessed = dto.ProviderTimeStamps[FriendlyName()];
                        }
                        else { trackDTO.lastAccessed = DateTime.Parse("01-01-1000"); }

                        TrackInfoService trackService = new TrackInfoService();
                        trackService.SetProvider(lfmProvider);
                        trackService.Fetch(trackDTO);

                        // handle error back from the provider
                        if (trackDTO.Status == WebServiceStatus.Error)
                        {
                            dto.Outcome = DataProviderOutcome.SystemError;
                            dto.Errors = new System.Collections.Generic.List<string> { "Web Service Error (" + trackDTO.Error + ") Artist (" + trackDTO.Artist + ") Track (" + trackDTO.Track + ")" };
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

            // otherwise refresh randomly (90% don't refresh)
            return (Rnd.Next(100) >= 90);
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
            // refresh strategy for Last.fm is complicated, this first bit is just to randomly 
            return RandomlyRefreshData(lastAccess);
        }
    }
}
