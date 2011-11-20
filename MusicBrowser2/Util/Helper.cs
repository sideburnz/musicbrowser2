using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.Win32;
using MusicBrowser.Providers;

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

        public enum knownType
        {
            Track,
            Playlist,
            Folder,
            Video,
            Image,
            Other
        }
        public static Dictionary<string, knownType> perceivedTypeCache = getKnownTypes();

        private static knownType determineType(string extension)
        {
            try
            {
                string pt = null;
                RegistryKey key = Registry.ClassesRoot;
                key = key.OpenSubKey(extension);
                if (key != null)
                {
                    pt = key.GetValue("PerceivedType") as string;
                }
                if (String.IsNullOrEmpty(pt))
                {
                    pt = key.GetValue("MediaCenter.16.PerceivedType.BAK") as string; // J. River fix
                }
                if (pt == null)
                {
                    pt = String.Empty;
                }
                pt = pt.ToLower();

                lock (perceivedTypeCache)
                {
                    switch (pt)
                    {
                        case "video":
                            if (Config.GetInstance().GetBooleanSetting("EnableExperimentalVideoSupport"))
                            {
                                perceivedTypeCache.Add(extension, knownType.Video);
                            }
                            else
                            {
                                perceivedTypeCache.Add(extension, knownType.Other);
                            }
                            break;
                        case "image":
                            if (Config.GetInstance().GetBooleanSetting("EnableExperimentalPhotoSupport"))
                            {
                                perceivedTypeCache.Add(extension, knownType.Image);
                            }
                            else
                            {
                                perceivedTypeCache.Add(extension, knownType.Other);
                            }
                            break;
                        case "audio":
                            perceivedTypeCache.Add(extension, knownType.Track);
                            break;
                        default:
                            perceivedTypeCache.Add(extension, knownType.Other);
                            break;
                    }
                }
            }
            catch
            {
                // if there's a problem, return an unknown type
                return knownType.Other;
            }
            return perceivedTypeCache[extension];
        }

        public static knownType getKnownType(FileSystemItem item)
        {
            // if it's a folder, don't worry about the type cache
            if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return knownType.Folder;
            }

            string extension = System.IO.Path.GetExtension(item.Name).ToLower();
            knownType itemType;
            // try to get the item from the type cache
            if (perceivedTypeCache.TryGetValue(extension, out itemType))
            {
                return itemType;
            }
            return determineType(extension);
        }

        private static Dictionary<string, knownType> getKnownTypes()
        {
            Dictionary<string,knownType> retVal = new Dictionary<string,knownType>();
            IEnumerable<string> extentions;

            extentions = Config.GetInstance().GetListSetting("Extensions.Playlist");
            foreach (string extention in extentions)
            {
                retVal.Add(extention, knownType.Playlist);
            }

            extentions = Config.GetInstance().GetListSetting("Extensions.Ignore");
            foreach (string extention in extentions)
            {
                retVal.Add(extention, knownType.Other);
            }
            // special circumstance
            retVal.Add(String.Empty, knownType.Other);

            return retVal;
        }

        public static string outputTypes()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("Known File Types: ");
            foreach (string k in perceivedTypeCache.Keys)
            {
                s.AppendLine(k + " " + perceivedTypeCache[k].ToString());
            }
            return s.ToString();
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

        static public string ImageCacheFullName(string key, string imageType)
        {
            string path = Config.GetInstance().GetStringSetting("Cache.Path") + "\\Images\\" + imageType + "\\" + key.Substring(0, 2);
            Directory.CreateDirectory(path);
            return path + "\\" + key + ".jpg";
        }

        public static string IBNPath(string category, string title)
        {
            return System.IO.Path.Combine(System.IO.Path.Combine(Util.Config.GetInstance().GetStringSetting("ImagesByName"), category), title);
 
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

        /// <summary>
        /// Compute the distance between two strings.
        /// adapted from http://www.dotnetperls.com/levenshtein
        /// </summary>
        public static int Levenshtein(string s, string t)
        {
            s = Cleanse(s);
            t = Cleanse(t);

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0) { return m; }
            if (m == 0) { return n; }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static string Cleanse(string raw)
        {
            string a = raw.ToLower();
            if (a.StartsWith("the ")) { return a.Substring(4).Trim(); }
            return a;
        }
    }
}
