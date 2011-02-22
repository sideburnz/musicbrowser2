using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TagLib;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System.Drawing;

namespace MusicBrowser.Providers.Metadata
{
    class TagSharpMetadataProvider : IBackgroundTaskable, IMetadataProvider
    {
        private const string MARKER = "TAGSHARP";
        private readonly IEntity _entity;
        private readonly IEntityCache _cache;

        public TagSharpMetadataProvider() { }

        public TagSharpMetadataProvider(IEntity entity, IEntityCache cache)
        {
            _entity = entity;
            _cache = cache;
        }

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {

            if (!entity.Kind.Equals(EntityKind.Song)) { return entity; }
            //TODO: replace this with checks for individual fields
            if (entity.Properties.ContainsKey(MARKER)) { return entity; }

            Logging.Logger.Verbose("TagSharpMetadataProvider.Fetch(" + entity.Path + ")", "start");

            try
            {
                TagLib.File fileTag = TagLib.File.Create(entity.Path);

                if (!String.IsNullOrEmpty(fileTag.Tag.Title.Trim()))
                {
                    entity.Title = fileTag.Tag.Title.Trim();
                    if (!entity.Properties.ContainsKey("album")) { entity.Properties.Add("album", fileTag.Tag.Album.Trim()); }
                    if (!entity.Properties.ContainsKey("artist")) { entity.Properties.Add("artist", fileTag.Tag.FirstPerformer.Trim()); }
                    
                    //if (!entity.Properties.ContainsKey("album")) { 
                    entity.Properties.Add("year", fileTag.Tag.Year.ToString());
                    entity.Properties.Add("disc", fileTag.Tag.Disc.ToString());
                    entity.Properties.Add("codec", fileTag.MimeType.Substring(7));
                    entity.Duration = (Int32)fileTag.Properties.Duration.TotalSeconds;
                    entity.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;
                    entity.Properties.Add("MusicBrainzArtist", fileTag.Tag.MusicBrainzReleaseArtistId);
                    entity.Properties.Add("MusicBrainzAlbum", fileTag.Tag.MusicBrainzReleaseId);

                    entity.Properties.Add("track", string.Format("{0:D2}", fileTag.Tag.Track));
                    if (Util.Config.getInstance().getBooleanSetting("PutDiscInTrackNo"))
                    {
                        if (fileTag.Tag.Disc != 0)
                        {
                            entity.Properties["track"] = string.Format("{0}.{1:D2}", fileTag.Tag.Disc, fileTag.Tag.Track);
                        }
                    }

                    if (!Util.Config.getInstance().getBooleanSetting("UseFolderImageForTracks"))
                    {
                        // cache the thumb
                        string tmpThumb = Util.Helper.ImageCacheFullName(entity.CacheKey, "Covers");
                        foreach (TagLib.IPicture pic in fileTag.Tag.Pictures)
                        {
                            if (!pic.Data.IsEmpty)
                            {
                                Bitmap bitmap = ImageProvider.Convert(pic);
                                bitmap = ImageProvider.Resize(bitmap, ImageType.Thumb);
                                ImageProvider.Save(bitmap, tmpThumb);

                                entity.IconPath = tmpThumb;
                                break;
                            }
                        }
                    }
                }
                entity.Properties.Add(MARKER, MARKER);
            }
            catch (Exception e) { Logging.Logger.Error(e); }

            entity.Dirty = true;
            return entity;
        }

        #endregion

        #region IBackgroundTaskable Members

        public string Title
        {
            get { return this.GetType().ToString(); }
        }

        public void Execute()
        {
            IEntity e = Fetch(_entity);
            _cache.Update(e.CacheKey, e);
        }

        #endregion

    }
}
