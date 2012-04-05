using System;
using MusicBrowser.Entities;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Engines.Metadata
{
    class LastFMTrackMetadataProvider : baseMetadataProvider
    {

        public LastFMTrackMetadataProvider()
        {
            Name = "LastFMTrackMetadataProvider";
            MinDaysBetweenHits = 7;
            MaxDaysBetweenHits = 100;
            RefreshPercentage = 10;
        }

        public override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders")) { return false; }
            if (String.IsNullOrEmpty(dto.Title)) { return false; }
            return true;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return dto.InheritsFrom<Track>();
        }

        public override ProviderOutcome DoWork(baseEntity dto)
        {
            WebServiceProvider lfmProvider = new LastFMWebProvider();
            Track workingDTO = (Track)dto;

            TrackInfoDTO trackDTO = new TrackInfoDTO();
            trackDTO.Track = workingDTO.Title;
            trackDTO.Artist = workingDTO.Artist;
            //trackDTO.MusicBrainzID = workingDTO.MusicBrainzID;
            trackDTO.Username = (Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
            trackDTO.Language = Util.Localization.LanguageNameToCode(Util.Config.GetInstance().GetStringSetting("Language"));
            if (workingDTO.MetadataStamps.ContainsKey(FriendlyName()))
            {
                trackDTO.LastAccessed = dto.MetadataStamps[FriendlyName()];
            }
            else { trackDTO.LastAccessed = DateTime.Parse("01-01-1000"); }

            TrackInfoService trackService = new TrackInfoService();
            trackService.SetProvider(lfmProvider);
            trackService.Fetch(trackDTO);

            // handle error back from the provider
            if (trackDTO.Status == WebServiceStatus.Error)
            {
                return ProviderOutcome.SystemError;
            }
            if (trackDTO.Status == WebServiceStatus.Warning)
            {
                return ProviderOutcome.NoData;
            }

            if (trackDTO.Plays > workingDTO.TimesPlayed)
            {
                workingDTO.TimesPlayed = trackDTO.Plays;
            }
            workingDTO.Listeners = trackDTO.Listeners;
            workingDTO.LastFMPlayCount = trackDTO.TotalPlays;

            workingDTO.Title = trackDTO.Track;
            workingDTO.Overview = trackDTO.Summary;
            workingDTO.MusicBrainzID = trackDTO.MusicBrainzID;
            workingDTO.Loved = trackDTO.Loved;

            dto = workingDTO;
            return ProviderOutcome.Success;
        }
    }
}
