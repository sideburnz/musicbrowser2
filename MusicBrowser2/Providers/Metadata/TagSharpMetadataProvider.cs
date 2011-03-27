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
            IDictionary<string, string> _props = entity.Properties;
            IEntity _parent = entity.Parent;

            if (!entity.Kind.Equals(EntityKind.Song)) { return entity; }
            //TODO: replace this with checks for individual fields
            if (entity.Properties.ContainsKey(MARKER)) { return entity; }
#if DEBUG
            Logging.Logger.Verbose("TagSharpMetadataProvider.Fetch(" + entity.Path + ")", "start");
#endif
            try
            {
                TagLib.File fileTag = TagLib.File.Create(entity.Path);

                if (!String.IsNullOrEmpty(fileTag.Tag.Title.Trim()))
                {
                    entity.Title = fileTag.Tag.Title.Trim();
                    if (!_props.ContainsKey("album")) { _props.Add("album", fileTag.Tag.Album.Trim()); }
                    if (!_props.ContainsKey("artist")) { _props.Add("artist", fileTag.Tag.FirstPerformer.Trim()); }
                    
                    //if (!entity.Properties.ContainsKey("album")) { 
                    _props.Add("release", fileTag.Tag.Year.ToString());
                    _props.Add("disc", fileTag.Tag.Disc.ToString());
                    _props.Add("codec", fileTag.MimeType.Substring(7));
                    entity.Duration = (Int32)fileTag.Properties.Duration.TotalSeconds;
                    entity.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;
                    _props.Add("MusicBrainzArtist", fileTag.Tag.MusicBrainzReleaseArtistId);
                    _props.Add("MusicBrainzAlbum", fileTag.Tag.MusicBrainzReleaseId);

                    // populate up
                    if (!(_parent == null))
                    {
                        if (_parent.Kind.Equals(EntityKind.Album))
                        {
                            if (string.IsNullOrEmpty(_parent.MusicBrainzID)) { _parent.MusicBrainzID = fileTag.Tag.MusicBrainzReleaseId; _parent.Dirty = true; }
                            if (!_parent.Properties.ContainsKey("release")) { _parent.Properties.Add("release", fileTag.Tag.Year.ToString()); _parent.Dirty = true; }
                        }
                        if (!(_parent.Parent == null))
                        {
                            if (_parent.Parent.Kind.Equals(EntityKind.Artist))
                            {
                                if (string.IsNullOrEmpty(_parent.Parent.MusicBrainzID)) { _parent.Parent.MusicBrainzID = fileTag.Tag.MusicBrainzReleaseArtistId; _parent.Parent.Dirty = true; }
                            }
                        }
                    }

                    _props.Add("track", string.Format("{0:D2}", fileTag.Tag.Track));
                    if (Util.Config.getInstance().getBooleanSetting("PutDiscInTrackNo"))
                    {
                        if (fileTag.Tag.Disc != 0)
                        {
                            _props["track"] = string.Format("{0}.{1:D2}", fileTag.Tag.Disc, fileTag.Tag.Track);
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
