using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using MusicBrowser.Util;
using System.Drawing.Drawing2D;

namespace MusicBrowser.Providers
{
    public enum ImageType
    {
        Thumb,
        Backdrop,
        Banner,
        Other
    }

    public enum ImageRatio
    {
        Ratio2to3,
        Ratio1to1,
        Ratio16to9,
        Ratio11to2,
        RatioUncommon
    }

    public static class ImageProvider
    {
        const int Thumbsize = 200;

        public static ImageRatio Ratio(Bitmap bitmap)
        {
            double ratio = (double)bitmap.Height / (double)bitmap.Width;

            if (ratio < 0.35)
            {
                return ImageRatio.Ratio11to2; // 0.1818181818181818
            }
            if (ratio >= 0.35 && ratio < 0.75) 
            {
                return ImageRatio.Ratio16to9; // 0.5625
            }
            if (ratio >= 0.75 && ratio < 1.25)
            {
                return ImageRatio.Ratio1to1; // 1
            }
            if (ratio >= 1.25 && ratio < 1.75)
            {
                return ImageRatio.Ratio2to3;  // 1.5
            }

            return ImageRatio.RatioUncommon;
        }

        public static Bitmap Convert(TagLib.IPicture picture)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                byte[] bytes = picture.Data.Data;
                ms.Write(bytes, 0, bytes.Length);
                return new Bitmap(ms);
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap Resize(Bitmap bitmap, ImageType type)
        {
            if (!type.Equals(ImageType.Thumb)) { return bitmap; }

            if (bitmap.Width < Thumbsize) { return bitmap; }
            if (bitmap.Height < Thumbsize) { return bitmap; }

            Bitmap b = new Bitmap(Thumbsize, Thumbsize);

            try
            {
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, 0, 0, Thumbsize, Thumbsize);
                g.Dispose();
            }
            catch { }
            return b;
        }

        public static bool Save(Icon bitmap, string filename)
        {
            if (bitmap == null) { return false; }

            EncoderParameters parms = new EncoderParameters(1);
            parms.Param[0] = new EncoderParameter(Encoder.Quality, Int64.Parse("90"));

            ImageCodecInfo codec = null;
            foreach (ImageCodecInfo codectemp in ImageCodecInfo.GetImageDecoders())
            {
                if (codectemp.MimeType == "image/jpeg")
                {
                    codec = codectemp;
                    break;
                }
            }

            try
            {
                // this fails on rare occassions for reasons I don't know
                // if that happens just don't save and let the item refresh in due course
                bitmap.ToBitmap().Save(filename, codec, parms);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Save(Image bitmap, string filename)
        {
            if (bitmap == null) { return false; }

            EncoderParameters parms = new EncoderParameters(1);
            parms.Param[0] = new EncoderParameter(Encoder.Quality, Int64.Parse("90"));

            ImageCodecInfo codec = null;
            foreach (ImageCodecInfo codectemp in ImageCodecInfo.GetImageDecoders())
            {
                if (codectemp.MimeType == "image/jpeg")
                {
                    codec = codectemp;
                    break;
                }
            }

            try
            {
                // this fails on rare occassions for reasons I don't know
                // if that happens just don't save and let the item refresh in due course
                bitmap.Save(filename, codec, parms);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        static IEnumerable<string> _images;

        public static string LocateFanArt(string path, ImageType type)
        {
            if (type == ImageType.Backdrop) { return internalFanArtSearch(path, "backdrop"); }
            if (type == ImageType.Banner) { return internalFanArtSearch(path, "banner"); }
            if (type == ImageType.Thumb)
            {
                string iconPath = internalFanArtSearch(path, "folder");
                if (string.IsNullOrEmpty(iconPath)) 
                {
                    iconPath = internalFanArtSearch(path, "cover");
                }
                return iconPath;
            }
            return string.Empty;
        }

        public static List<string> LocateBackdropList(string path)
        {
            List<string> backs = new List<string>();

            string iconPath = internalFanArtSearch(path, "backdrop");
            if (String.IsNullOrEmpty(iconPath)) { return backs; }
            backs.Add(iconPath);

            for (int i = 1; i < 10; i++)
            {
                iconPath = internalFanArtSearch(path, "backdrop" + i);
                if (String.IsNullOrEmpty(iconPath)) { return backs; }
                backs.Add(iconPath);
            }
            return backs;
        }

        private static string internalFanArtSearch(string path, string filename)
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

        public static Bitmap Load(string path)
        {
            try
            {
                return new Bitmap(path);
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
            int red = 0;
            int green = 0;
            int blue = 0;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;
            
            int count = width * height - dropped;
            if (count == 0) 
            {
                return System.Drawing.Color.FromArgb(128, 128, 128);
            }

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
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

            return System.Drawing.Color.FromArgb(avgR, avgG, avgB);
        }
    }
}
