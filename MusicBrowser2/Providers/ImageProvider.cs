using System;
using System.Collections.Generic;
using System.Drawing;
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
        Other
    }

    public static class ImageProvider
    {
        const int Thumbsize = 200;

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

            Bitmap result = new Bitmap(Thumbsize, Thumbsize);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(bitmap, 0, 0, Thumbsize, Thumbsize);
            return result;
        }

        public static void Save(Image bitmap, string filename)
        {
            if (bitmap == null) { return; }

            EncoderParameters parms = new EncoderParameters(1);
            parms.Param[0] = new EncoderParameter(Encoder.Quality, Int64.Parse("95"));

            ImageCodecInfo codec = null;
            foreach (ImageCodecInfo codectemp in ImageCodecInfo.GetImageDecoders())
            {
                if (codectemp.MimeType == "image/jpeg")
                {
                    codec = codectemp;
                    break;
                }
            }

            bitmap.Save(filename, codec, parms);
        }

        static IEnumerable<string> _images;

        public static string LocateFanArt(string path, ImageType type)
        {
            string filename = "folder";
            if (type == ImageType.Backdrop) { filename = "backdrop"; }
            if (_images == null) { _images = StandingData.GetStandingData("images"); }
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
    }
}
