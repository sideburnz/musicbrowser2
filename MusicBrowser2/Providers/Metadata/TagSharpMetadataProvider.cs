using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider  : IDataProvider
    {

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            Logging.Logger.Debug("Tag# " + dto.Path);

            #region killer questions

            if (Directory.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Path is a folder: " + dto.Path };
                return dto;
            }

            if (!File.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "File not found: " + dto.Path };
                return dto;
            }

            #endregion

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

                //if (fileTag.Tag.Performers != null) { dto.Performers.AddRange(fileTag.Tag.Performers); }
                //if (fileTag.Tag.Genres != null) { dto.Genres.AddRange(fileTag.Tag.Genres); }

                // cache the thumb
                foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                {
                    if (!pic.Data.IsEmpty)
                    {
                        dto.ThumbImage = ImageProvider.Convert(pic);
                        Logging.Logger.Debug("image obtained via tags");
                        break;
                    }
                }

                dto.Outcome = DataProviderOutcome.Success;
            }
            else
            {
                dto.Outcome = DataProviderOutcome.NotFound;
                dto.Errors = new List<string> { "No data found in tags" };
            }

            return dto;
        }
    }
}
