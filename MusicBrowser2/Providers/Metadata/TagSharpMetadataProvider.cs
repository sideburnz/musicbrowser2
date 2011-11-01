using System;
using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider : IDataProvider
    {
        private const string Name = "TagSharpMetadataProvider";

        private const int MinDaysBetweenHits = 180;
        private const int MaxDaysBetweenHits = 360;
        private const int RefreshPercentage = 25;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public string FriendlyName() { return Name; }

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            if (dto.DataType != DataTypes.Track)
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a track: " + dto.Path };
                return dto;
            }

            #endregion

            Statistics.Hit(Name + ".hit");

            try
            {
                TagLib.File fileTag = TagLib.File.Create(dto.Path);

                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    dto.TrackName = fileTag.Tag.Title;
                    dto.AlbumName = fileTag.Tag.Album;
                    dto.ArtistName = fileTag.Tag.FirstPerformer;
                    dto.AlbumArtist = fileTag.Tag.FirstAlbumArtist;
                    dto.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year);
                    dto.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);
                    dto.TrackNumber = Convert.ToInt32(fileTag.Tag.Track);
                    dto.Codec = fileTag.MimeType.Substring(7).ToLower();
                    dto.Duration = Convert.ToInt32(fileTag.Properties.Duration.TotalSeconds);
                    dto.MusicBrainzId = fileTag.Tag.MusicBrainzTrackId;

                    if (fileTag.Tag.Performers != null) { dto.Performers.AddRange(fileTag.Tag.Performers); }
                    if (fileTag.Tag.Genres != null) { dto.Genre = fileTag.Tag.FirstGenre; }
                    if (string.IsNullOrEmpty(dto.AlbumArtist)) { dto.AlbumArtist = dto.ArtistName; }

                    // cache the thumb
                    foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                    {
                        if (!pic.Data.IsEmpty)
                        {
                            dto.ThumbImage = ImageProvider.Convert(pic);
                            break;
                        }
                    }
                }
                else
                {
                    dto.Outcome = DataProviderOutcome.NotFound;
                    dto.Errors = new List<string> { "No data found in tags" };
                }
            }
            catch
            {
                dto.Outcome = DataProviderOutcome.SystemError;
                dto.Errors = new List<string> { "Error processing file" };
            }

            return dto;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "track");
        }

        public static void FetchLite(Entity entity)
        {
            // this implements a cut-down version of the Tag# metadata fetcher
            // this is meant to be as quick as possible to not hold up the UI

            #region killer questions
            if (entity.ProviderTimeStamps.ContainsKey(Name)) { return; }
            if (!entity.Kind.Equals(EntityKind.Track)) { return; }
            #endregion

            try
            {
                TagLib.File fileTag = TagLib.File.Create(entity.Path);
                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    entity.TrackName = fileTag.Tag.Title;
                    entity.AlbumName = fileTag.Tag.Album;
                    entity.ArtistName = fileTag.Tag.FirstPerformer;
                    entity.AlbumArtist = fileTag.Tag.FirstAlbumArtist;
                    entity.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year);
                    entity.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);
                    entity.TrackNumber = Convert.ToInt32(fileTag.Tag.Track);
                    entity.Codec = fileTag.MimeType.Substring(7).ToLower();
                    entity.Duration = Convert.ToInt32(fileTag.Properties.Duration.TotalSeconds);
                    entity.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;

                    if (string.IsNullOrEmpty(entity.AlbumArtist)) { entity.AlbumArtist = entity.ArtistName; }
                }
            }
            catch { }
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

            // otherwise refresh randomly (% don't refresh each run)
            return (Rnd.Next(100) >= RefreshPercentage);
        }

        public bool isStale(DateTime lastAccess)
        {
            return RandomlyRefreshData(lastAccess);
        }

        public ProviderType Type
        {
            get { return ProviderType.Core; }
        }
    }
}
