using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities.Kinds
{
    class Album : IEntity
    {
        public Album()
        {
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageAlbum";
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
                    string temp;
                    temp = ImageProvider.locateFanArt(Path, ImageType.Thumb);
                    if (!String.IsNullOrEmpty(temp))
                    {
                        IconPath = Util.Helper.ImageCacheFullName(CacheKey, "Thumbs");
                        ImageProvider.Save(
                            ImageProvider.Resize(
                            ImageProvider.Load(temp),
                            ImageType.Thumb),
                            IconPath);
                        Dirty = true;
                    }
                }
                if (string.IsNullOrEmpty(BackgroundPath))
                {
                    string temp;
                    temp = ImageProvider.locateFanArt(Path, ImageType.Backdrop);
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

        public override void CalculateValues()
        {
            StringBuilder sb = new StringBuilder();

            if (base.Children == 1) { sb.Append("1 Track  "); }
            if (base.Children > 1) { sb.Append(base.Children + " Tracks  "); }
            if (Duration > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(Duration);
                if (t.Hours == 0)
                {
                    sb.Append(string.Format("{0}:{1:D2}", (Int32)Math.Floor(t.TotalMinutes), t.Seconds));
                }
                else
                {
                    sb.Append(string.Format("{0}:{1:D2}:{2:D2}", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds));
                }
            }
            if (Properties.ContainsKey("year")) { sb.Append(Properties["year"] + "  "); }

            if (sb.Length > 0) { base.ShortSummaryLine1 = "Album  (" + sb.ToString().Trim() + ")"; }
            base.CalculateValues();
        }

        public override IEntity Parent
        {
            get { return base.Parent; }
            set
            {
                base.Parent = value;
                if (Properties.ContainsKey("artist")) { return; }
                if (value.Kind.CompareTo(EntityKind.Artist) == 0)
                {
                    Properties.Add("artist", value.Title);
                    Dirty = true;
                }
            }
        }
    }
}
