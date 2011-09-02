﻿using System;
using System.Text;
using MusicBrowser.Providers;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities.Kinds
{
    [DataContract]
    [KnownType(typeof(Album))]
    class Album : IEntity
    {
        public override string DefaultIconPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageAlbum"; }
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Album; }
        }

        public override string Path
        {
            get { return base.Path; }
            set
            {
                base.Path = value;

                if (string.IsNullOrEmpty(IconPath))
                {
                    string temp = ImageProvider.LocateFanArt(Path, ImageType.Thumb);
                    if (!String.IsNullOrEmpty(temp))
                    {
                        IconPath = Util.Helper.ImageCacheFullName(CacheKey, "Thumbs");
                        ImageProvider.Save(
                            ImageProvider.Resize(
                            ImageProvider.Load(temp),
                            ImageType.Thumb),
                            IconPath);
                    }
                }
                if (string.IsNullOrEmpty(BackgroundPath))
                {
                    string temp = ImageProvider.LocateFanArt(Path, ImageType.Backdrop);
                    if (!String.IsNullOrEmpty(temp)) { BackgroundPath = temp; }
                }
            }
        }

        public override string ShortSummaryLine1
        {
            get
            {
                if (string.IsNullOrEmpty(base.ShortSummaryLine1))
                {
                    return Kind.ToString();
                }
                return base.ShortSummaryLine1;
            }
            set { base.ShortSummaryLine1 = value; }
        }

        public override void UpdateValues()
        {
            StringBuilder sb = new StringBuilder();

            if (TrackCount == 1) { sb.Append("1 Track  "); }
            if (TrackCount > 1) { sb.Append(TrackCount + " Tracks  "); }
            if (Duration > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(Duration);
                if (t.Hours == 0)
                {
                    sb.Append(string.Format("{0}:{1:D2}  ", (Int32)Math.Floor(t.TotalMinutes), t.Seconds));
                }
                else
                {
                    sb.Append(string.Format("{0}:{1:D2}:{2:D2}  ", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds));
                }
            }
            if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { sb.Append(ReleaseDate.ToString("yyyy") + "  "); }

            if (sb.Length > 0) { base.ShortSummaryLine1 = "Album  (" + sb.ToString().Trim() + ")"; }
            base.UpdateValues();
        }
    }
}
