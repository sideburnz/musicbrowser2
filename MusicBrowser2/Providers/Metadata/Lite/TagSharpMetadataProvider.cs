using System;
using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;
using System.Drawing;

namespace MusicBrowser.Providers.Metadata.Lite
{
    public class TagSharpMetadataProvider
    {

        public static void FetchLite(baseEntity entity)
        {
            // this implements a cut-down version of the Tag# metadata fetcher
            // this is meant to be as quick as possible to not hold up the UI

            #region killer questions
            if (!entity.InheritsFrom<Track>()) { return; }
            #endregion

            Track track = (Track)entity;

            try
            {
                TagLib.File fileTag = TagLib.File.Create(track.Path);
                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    track.Title = fileTag.Tag.Title;
                    track.Album = fileTag.Tag.Album;
                    track.Artist = fileTag.Tag.FirstPerformer;
                    track.AlbumArtist = fileTag.Tag.FirstAlbumArtist;
                    track.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year);
                    track.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);
                    track.TrackNumber = Convert.ToInt32(fileTag.Tag.Track);
                    track.Codec = fileTag.MimeType.Substring(7).ToLower();
                    track.Duration = Convert.ToInt32(fileTag.Properties.Duration.TotalSeconds);
                    track.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;

                    if (fileTag.Tag.Genres != null) { track.Genres = fileTag.Tag.Genres; }

                    if (string.IsNullOrEmpty(track.AlbumArtist)) { track.AlbumArtist = track.Artist; }

                    foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                    {
                        if (!pic.Data.IsEmpty)
                        {
                            Bitmap Thumb = ImageProvider.Convert(pic);
                            if (Thumb != null)
                            {
                                string iconPath = Util.Helper.ImageCacheFullName(track.CacheKey, "Thumbs");
                                if (ImageProvider.Save(ImageProvider.Resize(Thumb, ImageType.Thumb), iconPath))
                                {
                                    track.ThumbPath = iconPath;
                                }
                            }
                            break;
                        }
                    }

                    entity = track;
                }
            }
            catch { }
        }
    }
}
