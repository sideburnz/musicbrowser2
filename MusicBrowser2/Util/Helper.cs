using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using MusicBrowser.Providers;
using System.Security.Cryptography;

namespace MusicBrowser.Util
{
    public static class Helper
    {
        #region application folders

        static string _appFolder;
        public static string AppFolder
        {
            get
            {
                if (_appFolder == null)
                {
                    var e = Path.Combine(Environment.GetEnvironmentVariable("ProgramData"), "MusicBrowser");
                    if (!Directory.Exists(e))
                    {
                        try
                        {
                            Directory.CreateDirectory(e);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Application folder for MusicBrowser is missing: " + e, ex);
                        }
                    }
                    _appFolder = e;
                }
                return _appFolder;
            }
        }

        static string _appConfigFile; 
        public static string AppConfigFile
        {
            get { return _appConfigFile ?? (_appConfigFile = Path.Combine(AppFolder, "MusicBrowser.config")); }
        }

        static string _appCachePath;
        public static string AppCachePath
        {
            get
            {
                if (_appCachePath == null)
                {
                    var e = Path.Combine(AppFolder, "Cache");
                    if (!Directory.Exists(e))
                    {
                        try
                        {
                            Directory.CreateDirectory(e);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Cache folder for MusicBrowser is missing: " + e, ex);
                        }
                    }
                    try
                    {
                        Directory.CreateDirectory(e + "\\Images");
                        Directory.CreateDirectory(e + "\\Images\\Backgrounds");
                        Directory.CreateDirectory(e + "\\Images\\Covers");
                        Directory.CreateDirectory(e + "\\Images\\Thumbs");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Image cache folder for MusicBrowser is missing: " + e + "\\Images", ex);
                    }
                    if (!Directory.Exists(e + "\\Entities"))
                    {
                        try
                        {
                            Directory.CreateDirectory(e + "\\Entities");
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Entity cache folder for MusicBrowser is missing: " + e + "\\Entities", ex);
                        }
                    }
                    _appCachePath = e;
                }
                return _appCachePath;
            }
        }

        static string _appLogFolder;
        public static string AppLogFolder
        {
            get
            {
                if (_appLogFolder == null)
                {
                    var e = Path.Combine(AppFolder, "Logs");
                    if (!Directory.Exists(e))
                    {
                        try
                        {
                            Directory.CreateDirectory(e);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Log folder for MusicBrowser is missing: " + e, ex);
                        }
                    }
                    _appLogFolder = e;
                }
                return _appLogFolder;
            }
        }

        static string _PlugInFolder;
        public static string PlugInFolder
        {
            get
            {
                if (_PlugInFolder == null)
                {
                    var e = Path.Combine(AppFolder, "PlugIn");
                    if (!Directory.Exists(e))
                    {
                        Directory.CreateDirectory(e);
                    }
                    _PlugInFolder = e;
                }
                return _PlugInFolder;
            }
        }

        #endregion

        #region file identifiers
        static IEnumerable<string> _dicSongExts;
        public static bool IsSong(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            if (_dicSongExts == null)
            {
                _dicSongExts = StandingData.GetStandingData("songs");
            }
            return _dicSongExts.Any(item => extension == item);
        }

        static IEnumerable<string> _dicPlExts;
        public static bool IsPlaylist(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            if (_dicPlExts == null)
            {
                _dicPlExts = StandingData.GetStandingData("playlists");
            }
            return _dicPlExts.Any(item => extension == item);
        }

        static IEnumerable<string> _dicImgExts;
        public static bool IsImage(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (_dicImgExts == null)
            {
                _dicImgExts = StandingData.GetStandingData("images");
            }
            return _dicImgExts.Any(item => extension == item);
        }

        static IEnumerable<string> _dicNEExts;
        public static bool IsNotEntity(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (_dicNEExts == null)
            {
                _dicNEExts = StandingData.GetStandingData("nonentityextentions");
            }
            return _dicNEExts.Any(item => extension == item);
        }
        

        public static bool IsFolder(FileAttributes attr)
        {
            return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
        }
        #endregion

        public static XmlNode CreateXmlNode(XmlDocument parent, string name, string value)
        {
            XmlNode node = parent.CreateNode(XmlNodeType.Element, name, "");
            node.InnerText = value;
            return node;
        }

        public static string ReadXmlNode(XmlDocument parent, string xpath)
        {
            try
            {
                return parent.SelectSingleNode(xpath).InnerText;
            }
            catch { }
            return string.Empty;
        }

        static public string GetCacheKey(string seed)
        {
            // keys are 64 bytes
            byte[] buffer = Encoding.ASCII.GetBytes(seed.ToLower());
            SHA256CryptoServiceProvider cryptoTransform = new SHA256CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransform.ComputeHash(buffer)).Replace("-", String.Empty);
            return hash;
        }

        static public string CacheFullName(string key)
        {
            return Helper.AppCachePath + "\\Entities\\" + key + ".xml";
        }

        static public string ImageCacheFullName(string key, string imagetype)
        {
            return Helper.AppCachePath + "\\Images\\" + imagetype + "\\" + key + ".jpg";
        }

        /// <summary>
        /// method to strip HTML tags using Regular Expressions
        /// </summary>
        /// <param name="str">String to strip HTML from</param>
        /// <returns></returns>
        public static string StripHTML(string str)
        {
            //variable to hold the returned value
            string strippedString;
            try
            {
                //variable to hold our RegularExpression pattern
                string pattern = "<.*?>";
                //replace all HTML tags
                strippedString = Regex.Replace(str, pattern, string.Empty);
            }
            catch
            {
                strippedString = string.Empty;
            }
            return strippedString.Replace("&quot;", "'").Replace("&amp;", "&");
        }

        public static long ParseVersion(string version)
        {
            long ret = 0;
            try
            {
                string[] parts;
                if (String.IsNullOrEmpty(version)) 
                { 
                    parts = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.'); 
                }
                else 
                { 
                    parts = version.Split('.'); 
                }
                ret = int.Parse(parts[3]) +
                    (int.Parse(parts[2]) * 1000) +
                    (int.Parse(parts[1]) * 1000 * 100) +
                    (int.Parse(parts[0]) * 1000 * 100 * 100);
            }
            catch { }
            return ret;
        }

    }
}
