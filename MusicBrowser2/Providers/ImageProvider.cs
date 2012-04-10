using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using MusicBrowser.Util;

namespace MusicBrowser.Providers
{
    public enum ImageType
    {
        Thumb,
        Backdrop,
        Banner,
        Logo
    }

    public enum ImageRatio
    {
        Ratio2To3,
        Ratio1To1,
        Ratio16To9,
        Ratio11To2,
        RatioUncommon
    }

    public static class ImageProvider
    {
        private const int Thumbsize = 300;

        public static ImageRatio Ratio(Bitmap bitmap)
        {
            double ratio = bitmap.Height / (double)bitmap.Width;

            if (ratio < 0.35)
            {
                return ImageRatio.Ratio11To2; // 0.1818181818181818
            }
            if (ratio >= 0.35 && ratio < 0.75) 
            {
                return ImageRatio.Ratio16To9; // 0.5625
            }
            if (ratio >= 0.75 && ratio < 1.25)
            {
                return ImageRatio.Ratio1To1; // 1
            }
            if (ratio >= 1.25 && ratio < 1.75)
            {
                return ImageRatio.Ratio2To3;  // 1.5
            }

            return ImageRatio.RatioUncommon;
        }

        public static Bitmap Resize(Bitmap bitmap, ImageType type)
        {
            if (!type.Equals(ImageType.Thumb)) { return bitmap; }

            if (bitmap.Width < Thumbsize) { return bitmap; }
            if (bitmap.Height < Thumbsize) { return bitmap; }

            Bitmap b = new Bitmap(Thumbsize, Thumbsize);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, 0, 0, Thumbsize, Thumbsize);
                g.Dispose();
            }
            catch { }
            return b;
        }

        public static void Save(Image bitmap, string filename)
        {
            if (bitmap == null) {
                return;
            }

            try
            {
                // this fails on rare occassions for reasons I don't know
                // if that happens just don't save and let the item refresh in due course
                bitmap.Save(filename, ImageFormat.Png);
            }
            catch {}
        }

        static IEnumerable<string> _images;

        public static string LocateFanArt(string path, ImageType type)
        {
            if (type == ImageType.Backdrop) { return InternalFanArtSearch(path, "backdrop"); }
            if (type == ImageType.Banner) { return InternalFanArtSearch(path, "banner"); }
            if (type == ImageType.Logo)
            {
                string logoPath = InternalFanArtSearch(path, "logo");
                if (logoPath.ToLower().EndsWith(@"\logo.png"))
                {
                    return logoPath;
                }
                return string.Empty;
            }
            if (type == ImageType.Thumb)
            {
                string iconPath = InternalFanArtSearch(path, "folder");
                if (string.IsNullOrEmpty(iconPath)) 
                {
                    iconPath = InternalFanArtSearch(path, "cover");
                }
                return iconPath;
            }
            return string.Empty;
        }

        public static List<string> LocateBackdropList(string path)
        {
            List<string> backs = new List<string>();

            string iconPath = InternalFanArtSearch(path, "backdrop");
            if (String.IsNullOrEmpty(iconPath)) { return backs; }
            backs.Add(iconPath);

            for (int i = 1; i < 99; i++)
            {
                iconPath = InternalFanArtSearch(path, "backdrop" + i);
                if (String.IsNullOrEmpty(iconPath)) { return backs; }
                backs.Add(iconPath);
            }
            return backs;
        }

        private static string InternalFanArtSearch(string path, string filename)
        {
            if (_images == null) { _images = Config.GetInstance().GetListSetting("Extensions.Image"); }
            foreach (string item in _images)
            {
                string tmp = string.Concat(path, "\\", filename, item);
                if (File.Exists(tmp)) { return tmp; }
            }
            return string.Empty;
        }

        public static Bitmap Download(string imageUrl, ImageType type)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
                return Resize(bitmap, type);
            }
            catch
            {
                return null;
            }
        }

        public static Color CalculateAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            const int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr scan0 = srcData.Scan0;
            
            int count = width * height - dropped;
            if (count == 0) 
            {
                return Color.FromArgb(128, 128, 128);
            }

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        int red = p[idx + 2];
                        int green = p[idx + 1];
                        int blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return Color.FromArgb(avgR, avgG, avgB);
        }
    }
}
