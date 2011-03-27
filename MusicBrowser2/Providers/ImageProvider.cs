using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MusicBrowser.Util;
using TagLib;
using System.Net;

namespace MusicBrowser.Providers
{
    public enum ImageType
    {
        Thumb,
        Backdrop,
        Other
    }

    public class ImageProvider
    {
        const int THUMBSIZE = 200;

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

            if (bitmap.Width < THUMBSIZE) { return bitmap; }
            if (bitmap.Height < THUMBSIZE) { return bitmap; }

            Bitmap result = new Bitmap(THUMBSIZE, THUMBSIZE);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(bitmap, 0, 0, THUMBSIZE, THUMBSIZE);
            return result;
        }

        public static void Save(Bitmap bitmap, string filename)
        {
            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        static IEnumerable<string> images = null;

        public static string locateFanArt(string path, ImageType type)
        {
            string filename = "folder";
            if (type == ImageType.Backdrop) { filename = "backdrop"; }
            if (images == null) { images = StandingData.GetStandingData("images"); }
            foreach (string item in images)
            {
                string tmp = string.Concat(path, "\\", filename, item);
                if (System.IO.File.Exists(tmp)) { return tmp; }
            }
            return string.Empty;
        }

        public static Bitmap Download(string imageURL, ImageType type)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageURL);
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

        //public static void SaveImage(Image image, string filename)
        //{
        //    try
        //    {
        //        EncoderParameters parms = new EncoderParameters(1);
        //        parms.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Int64.Parse("80"));
        //        ImageCodecInfo codec = null;
        //        foreach (ImageCodecInfo codectemp in ImageCodecInfo.GetImageDecoders())
        //        {
        //            if (codectemp.MimeType == "image/jpeg")
        //            {
        //                codec = codectemp;
        //                break;
        //            }
        //        }
        //        if (codec == null)
        //        {
        //            throw new Exception("Codec not found for image/jpeg");
        //        }
        //        image.Save(filename, codec, parms);
        //        Logging.Logger.Debug("saving image: " + filename);
        //    }
        //    catch (Exception e)
        //    {
        //        Logging.Logger.Error(e);
        //    }
        //}

    }
}
