using System;
using System.Drawing;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Engines.Metadata
{
    class LastFMArtistMetadataProvider : baseMetadataProvider
    {

        public LastFMArtistMetadataProvider()
        {
            Name = "LastFMArtistMetadataProvider";
            MinDaysBetweenHits = 7;
            MaxDaysBetweenHits = 100;
            RefreshPercentage = 10;
        }

        protected override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Util.Config.GetBooleanSetting("Internet.UseProviders")) { return false; }
            if (String.IsNullOrEmpty(dto.Title)) { return false; }
            return true;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return dto.InheritsFrom<Artist>();
        }

        protected override ProviderOutcome DoWork(baseEntity dto)
        {
            Artist workingDTO = (Artist)dto;
            WebServiceProvider lfmProvider = new LastFMWebProvider();

            ArtistInfoServiceDTO artistDTO = new ArtistInfoServiceDTO();
            artistDTO.Artist = workingDTO.Title;
            artistDTO.MusicBrainzID = workingDTO.MusicBrainzID;
            artistDTO.Username = (Util.Config.GetStringSetting("Internet.LastFMUserName"));
            artistDTO.Language = Util.Localization.LanguageNameToCode(Util.Config.GetStringSetting("Language"));
            if (dto.MetadataStamps.ContainsKey(FriendlyName()))
            {
                artistDTO.LastAccessed = dto.MetadataStamps[FriendlyName()];
            }
            else { artistDTO.LastAccessed = DateTime.Parse("01-01-1000"); }

            ArtistInfoService artistService = new ArtistInfoService();
            artistService.SetProvider(lfmProvider);
            artistService.Fetch(artistDTO);

            // handle error back from the provider
            if (artistDTO.Status == WebServiceStatus.Error)
            {
                return ProviderOutcome.SystemError;
            }
            if (artistDTO.Status == WebServiceStatus.Warning)
            {
                return ProviderOutcome.InvalidInput;
            }

            if (artistDTO.Plays > workingDTO.PlayState.TimesPlayed)
            {
                workingDTO.PlayState.TimesPlayed = artistDTO.Plays;
            }
            workingDTO.Listeners = artistDTO.Listeners;
            workingDTO.LastFMPlayCount = artistDTO.TotalPlays;

            workingDTO.Title = artistDTO.Artist;
            workingDTO.MusicBrainzID = artistDTO.MusicBrainzID;
            workingDTO.Overview = artistDTO.Summary;

            if (workingDTO.ThumbPath == workingDTO.DefaultThumbPath)
            {
                Bitmap thumb = ImageProvider.Download(artistDTO.Image, ImageType.Thumb);
                workingDTO.ThumbPath = Util.Helper.ImageCacheFullName(workingDTO.CacheKey, ImageType.Thumb, -1);
                if (!ImageProvider.Save(thumb, dto.ThumbPath))
                {
                    // if the save fails, forget this ever happened
                    dto.ThumbPath = String.Empty;
                }
            }

            dto = workingDTO;
            return ProviderOutcome.Success;
        }
    }
}
