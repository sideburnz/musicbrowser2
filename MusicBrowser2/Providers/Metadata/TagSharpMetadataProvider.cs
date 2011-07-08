using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider : IDataProvider
    {

        public DataProviderDTO GetDTO()
        {
            DataProviderDTO temp = new DataProviderDTO();

            temp.Parameters = new Dictionary<string, string>();
            temp.Parameters.Add("path", String.Empty);

            return temp;
        }

        public DataProviderDTO Fetch(DataProviderDTO input)
        {
            Logging.Logger.Debug("Tag# " + input.Parameters["path"]);

            DataProviderDTO result = new DataProviderDTO();
            result.Parameters = new Dictionary<string, string>();

            if (Directory.Exists(input.Parameters["path"]))
            {
                result.Outcome = DataProviderOutcome.InvalidInput;
                result.Errors = new List<string> {"Path is a folder" + input.Parameters["path"]};
                return result;
            }

            if (!File.Exists(input.Parameters["path"]))
            {
                result.Outcome = DataProviderOutcome.InvalidInput;
                result.Errors = new List<string> { "File not found" + input.Parameters["path"] };
                return result;
            }

            result.Outcome = DataProviderOutcome.Success;

            TagLib.File fileTag = TagLib.File.Create(input.Parameters["path"]);

            if (!String.IsNullOrEmpty(fileTag.Tag.Title))
            {
                result.Parameters.Add("title", fileTag.Tag.Title.Trim());
                result.Parameters.Add("album", fileTag.Tag.Album);
                result.Parameters.Add("artist", fileTag.Tag.FirstPerformer);
                result.Parameters.Add("albumartist", fileTag.Tag.FirstAlbumArtist);
                result.Parameters.Add("release", fileTag.Tag.Year.ToString());
                result.Parameters.Add("disc", fileTag.Tag.Disc.ToString());
                result.Parameters.Add("track", string.Format("{0:D2}", fileTag.Tag.Track));
                result.Parameters.Add("codec", fileTag.MimeType.Substring(7).ToLower());
                result.Parameters.Add("duration", fileTag.Properties.Duration.TotalSeconds.ToString());
                result.Parameters.Add("MusicBrainzID", fileTag.Tag.MusicBrainzTrackId);
                result.Parameters.Add("MusicBrainzArtist", fileTag.Tag.MusicBrainzReleaseArtistId);
                result.Parameters.Add("MusicBrainzAlbum", fileTag.Tag.MusicBrainzReleaseId);
                result.Parameters.Add("genres", fileTag.Tag.JoinedGenres);
                result.Parameters.Add("performers", fileTag.Tag.JoinedPerformers);


                //// cache the thumb
                //string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Covers");
                //foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                //{
                //    if (!pic.Data.IsEmpty)
                //    {
                //        Bitmap bitmap = ImageProvider.Convert(pic);
                //        bitmap = ImageProvider.Resize(bitmap, ImageType.Thumb);
                //        ImageProvider.Save(bitmap, tmpThumb);

                //        entity.IconPath = tmpThumb;
                //        break;
                //    }
                //}


            }
            else
            {
                result.Outcome = DataProviderOutcome.NotFound;
                result.Errors = new List<string>();
                result.Errors.Add("No data found in tags");
            }

            return result;
        }
    }
}
