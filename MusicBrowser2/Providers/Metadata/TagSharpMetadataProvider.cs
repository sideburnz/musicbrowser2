using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider  : IDataProvider
    {

        public DataProviderDTO Fetch(DataProviderDTO entity)
        {
            Logging.Logger.Debug("Tag# " + entity.Path);

            if (Directory.Exists(entity.Path))
            {
                entity.Outcome = DataProviderOutcome.InvalidInput;
                entity.Errors = new List<string> { "Path is a folder: " + entity.Path };
                return entity;
            }

            if (!File.Exists(entity.Path))
            {
                entity.Outcome = DataProviderOutcome.InvalidInput;
                entity.Errors = new List<string> { "File not found: " + entity.Path };
                return entity;
            }

            entity.Outcome = DataProviderOutcome.Success;

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
                entity.MusicBrainzId = fileTag.Tag.MusicBrainzTrackId;

//                input.Performers = fileTag.Tag.Performers;
//                input.Genres = fileTag.Tag.Genres;
            }

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


            //}
            else
            {
                entity.Outcome = DataProviderOutcome.NotFound;
                entity.Errors = new List<string> {"No data found in tags"};
            }

            return entity;
        }
    }
}
