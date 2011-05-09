using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TagLib;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System.Drawing;

namespace MusicBrowser.Providers.Metadata
{
    public class TagSharpMetadataProvider : IBackgroundTaskable, IMetadataProvider
    {
        private const string MARKER = "TAGSHARP";
        private readonly IEntity _entity;
        private readonly EntityCache _cache;

        public TagSharpMetadataProvider() { }

        public TagSharpMetadataProvider(IEntity entity, EntityCache cache)
        {
            _entity = entity;
            _cache = cache;
        }

        #region IMetadataProvider Members

        public IEntity Fetch(IEntity entity)
        {
            IEntity _parent = entity.Parent;

            if (!entity.Kind.Equals(EntityKind.Song)) { return entity; }
            if (entity.Properties.ContainsKey(MARKER)) { return entity; }
#if DEBUG
            Logging.Logger.Verbose("TagSharpMetadataProvider.Fetch(" + entity.Path + ")", "start");
#endif

            MusicBrowser.Providers.Statistics.GetInstance().Hit("tagsharp.hit");
            try
            {
                TagLib.File fileTag = TagLib.File.Create(entity.Path);

                if (!String.IsNullOrEmpty(fileTag.Tag.Title))
                {
                    entity.Title = fileTag.Tag.Title.Trim();
                    entity.SetProperty("album", fileTag.Tag.Album, false);
                    entity.SetProperty("artist", fileTag.Tag.FirstPerformer, false);
                    entity.SetProperty("albumartist", fileTag.Tag.FirstAlbumArtist, false);
                    
                    entity.SetProperty("release", fileTag.Tag.Year.ToString(), false);
                    entity.SetProperty("disc", fileTag.Tag.Disc.ToString(), false);
                    entity.SetProperty("track", string.Format("{0:D2}", fileTag.Tag.Track), false);
                    entity.SetProperty("codec", fileTag.MimeType.Substring(7).ToLower(), false);
                    entity.Duration = (Int32)fileTag.Properties.Duration.TotalSeconds;
                    entity.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;
                    entity.SetProperty("MusicBrainzArtist", fileTag.Tag.MusicBrainzReleaseArtistId, false);
                    entity.SetProperty("MusicBrainzAlbum", fileTag.Tag.MusicBrainzReleaseId, false);
                    entity.SetProperty("genres", fileTag.Tag.JoinedGenres, false);
                    entity.SetProperty("performers", fileTag.Tag.JoinedPerformers, false);
                    // populate up
                    if (!(_parent == null))
                    {
                        if (_parent.Kind.Equals(EntityKind.Album))
                        {
                            if (string.IsNullOrEmpty(_parent.MusicBrainzID)) 
                            { 
                                _parent.MusicBrainzID = fileTag.Tag.MusicBrainzReleaseId; 
                                _parent.Dirty = true; 
                            }
                            _parent.SetProperty("release", fileTag.Tag.Year.ToString(), false);
                        }
                        if (!(_parent.Parent == null))
                        {
                            if (_parent.Parent.Kind.Equals(EntityKind.Artist))
                            {
                                if (string.IsNullOrEmpty(_parent.Parent.MusicBrainzID)) 
                                {
                                    if (!String.IsNullOrEmpty(fileTag.Tag.FirstAlbumArtist))
                                    {
                                        _parent.Parent.Title = fileTag.Tag.FirstAlbumArtist;
                                        _parent.Parent.Dirty = true;
                                    }
                                }
                            }
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
                    entity.SetProperty(MARKER, DateTime.Now.ToString("yyyy-MMM-dd"), true);
                }
            }
    
            catch (Exception e) { Logging.Logger.Error(e); }

            entity.Dirty = true;
            entity.CalculateValues();
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
