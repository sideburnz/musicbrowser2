using System;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities.Kinds
{
    class Genre : IEntity
    {
        public Genre()
        {
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageQuestion";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Genre; }
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
                        Dirty = true;
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
    }
}
