using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
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

        public static string CachePath
        {
            get { return Path.Combine(AppFolder, "Cache"); }
        }

        public static void BuildCachePath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cache folder for MusicBrowser is missing: " + path, ex);
                }
            }
            try
            {
                Directory.CreateDirectory(path + "\\Images");
                Directory.CreateDirectory(path + "\\Images\\Backgrounds");
                Directory.CreateDirectory(path + "\\Images\\Covers");
                Directory.CreateDirectory(path + "\\Images\\Thumbs");
            }
            catch (Exception ex)
            {
                throw new Exception("Image cache folder for MusicBrowser is missing: " + path + "\\Images", ex);
            }
            if (!Directory.Exists(path + "\\Entities"))
            {
                try
                {
                    Directory.CreateDirectory(path + "\\Entities");
                }
                catch (Exception ex)
                {
                    throw new Exception("Entity cache folder for MusicBrowser is missing: " + path + "\\Entities", ex);
                }
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

        static string _plugInFolder;
        public static string PlugInFolder
        {
            get
            {
                if (_plugInFolder == null)
                {
                    var e = Path.Combine(AppFolder, "PlugIn");
                    if (!Directory.Exists(e))
                    {
                        Directory.CreateDirectory(e);
                    }
                    _plugInFolder = e;
                }
                return _plugInFolder;
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

        static IEnumerable<string> _dicNeExts;
        public static bool IsNotEntity(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (_dicNeExts == null)
            {
                _dicNeExts = StandingData.GetStandingData("nonentityextentions");
            }
            return _dicNeExts.Any(item => extension == item);
        }
        

        public static bool IsFolder(FileAttributes attributes)
        {
            return ((attributes & FileAttributes.Directory) == FileAttributes.Directory);
        }
        #endregion

        public static XmlNode CreateXmlNode(XmlDocument parent, string name, string value)
        {
            XmlNode node = parent.CreateNode(XmlNodeType.Element, name, "");
            node.InnerText = value;
            return node;
        }

        public static string ReadXmlNode(XmlDocument parent, string xPath)
        {
            return ReadXmlNode(parent, xPath, string.Empty);
        }

        public static  string ReadXmlNode(XmlDocument parent, string xPath, string defaultValue)
        {
            var selectSingleNode = parent.SelectSingleNode(xPath);
            if (selectSingleNode != null)
            {
                return selectSingleNode.InnerText;
            }
            return defaultValue;
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
            return Config.GetInstance().GetSetting("CachePath") + "\\Entities\\" + key + ".xml";
        }

        static public string ImageCacheFullName(string key, string imageType)
        {
            return Config.GetInstance().GetSetting("CachePath") + "\\Images\\" + imageType + "\\" + key + ".jpg";
        }

        /// <summary>
        /// method to strip HTML tags using Regular Expressions
        /// </summary>
        /// <returns></returns>
        public static string StripHTML(string raw)
        {
            //variable to hold the returned value
            string strippedString;
            try
            {
                //variable to hold our RegularExpression pattern
                const string pattern = "<.*?>";
                //replace all HTML tags
                strippedString = Regex.Replace(raw, pattern, string.Empty);
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

        // this measures the similarity of two strings
        // it's fairly rough and ready
        public static int Similarity(string string1, string string2)
        {
            int hits = 0;

            string[] pairs1 = new string[string1.Length - 1];
            for (int i = 0; i < string1.Length - 1; i++) { pairs1[i] = string1.Substring(i, 2); }

            if (pairs1.Count() == 0) { return 0; }

            string[] pairs2 = new string[string2.Length - 1];
            for (int i = 0; i < string2.Length - 1; i++) { pairs2[i] = string2.Substring(i, 2); }

            foreach (string s1 in pairs1)
            {
                foreach (string s2 in pairs2)
                {
                    if (s1 == s2)
                    {
                        hits++;
                        break;
                    }
                }
            }

            return (100 * (hits / pairs1.Count()));

        }
    }
}
