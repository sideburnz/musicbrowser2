using System;
using System.Drawing;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Engines.Metadata
{
    class LastFMAlbumMetadataProvider : baseMetadataProvider
    {

        public LastFMAlbumMetadataProvider()
        {
            Name = "LastFMAlbumMetadataProvider";
            MinDaysBetweenHits = 7;
            MaxDaysBetweenHits = 100;
            RefreshPercentage = 10;
        }

        public override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders")) { return false; }
            if (String.IsNullOrEmpty(dto.Title)) { return false; }
            if (String.IsNullOrEmpty(((Album)dto).AlbumArtist)) { return false; }
            return true;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return dto.InheritsFrom<Album>();
        }

        public override ProviderOutcome DoWork(baseEntity dto)
        {
            WebServiceProvider lfmProvider = new LastFMWebProvider();
            Album workingDTO = (Album)dto;

            AlbumInfoServiceDTO albumDTO = new AlbumInfoServiceDTO();
            albumDTO.Album = workingDTO.Title;
            albumDTO.MusicBrainzID = workingDTO.MusicBrainzID;
            albumDTO.Artist = workingDTO.AlbumArtist;
            albumDTO.Username = (Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
            albumDTO.Language = Util.Localization.LanguageNameToCode(Util.Config.GetInstance().GetStringSetting("Language"));
            if (dto.MetadataStamps.ContainsKey(FriendlyName()))
            {
                albumDTO.LastAccessed = dto.MetadataStamps[FriendlyName()];
            }
            else { albumDTO.LastAccessed = DateTime.Parse("01-01-1000"); }

            AlbumInfoService albumService = new AlbumInfoService();
            albumService.SetProvider(lfmProvider);
            albumService.Fetch(albumDTO);

            // handle error back from the provider
            if (albumDTO.Status == WebServiceStatus.Error)
            {
                return ProviderOutcome.SystemError;
            }
            if (albumDTO.Status == WebServiceStatus.Warning)
            {
                return ProviderOutcome.NoData;
            }

            if (albumDTO.Plays > workingDTO.TimesPlayed)
            {
                workingDTO.TimesPlayed = albumDTO.Plays;
            }
            workingDTO.Listeners = albumDTO.Listeners;
            workingDTO.LastFMPlayCount = albumDTO.TotalPlays;

            workingDTO.Title = albumDTO.Album;
            workingDTO.Overview = albumDTO.Summary;
            workingDTO.MusicBrainzID = albumDTO.MusicBrainzID;

            workingDTO.ReleaseDate = albumDTO.Release;

            if (workingDTO.ThumbPath == workingDTO.DefaultThumbPath)
            {
                Bitmap thumb = ImageProvider.Download(albumDTO.Image, ImageType.Thumb);
                workingDTO.ThumbPath = Util.Helper.ImageCacheFullName(workingDTO.CacheKey, ImageType.Thumb, -1);
                ImageProvider.Save(thumb, dto.ThumbPath);
            }

            dto = workingDTO;
            return ProviderOutcome.Success;
        }
    }
}

