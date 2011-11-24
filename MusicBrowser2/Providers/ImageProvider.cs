﻿using System;
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
    }
}
