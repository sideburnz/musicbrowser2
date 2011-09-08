using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.Win32;

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
            Song,
            Playlist,
            Other
        }
        public static Dictionary<string, knownType> perceivedTypeCache = null;

        public static bool IsSong(string filename)
        {
            if (perceivedTypeCache == null)
            {
                perceivedTypeCache = getKnownTypes();
            }

            string extension = System.IO.Path.GetExtension(filename).ToLower();

            knownType itemType;
            if (perceivedTypeCache.TryGetValue(extension, out itemType))
            {
                return itemType == knownType.Song;
            }
            return determineType(extension) == knownType.Song;            
        }

        public static bool IsPlaylist(string fileName)
        {
            if (perceivedTypeCache == null) 
            {
                perceivedTypeCache = getKnownTypes();
            }

            if (!System.IO.File.Exists(fileName)) return false;
            string extension = System.IO.Path.GetExtension(fileName).ToLower();

            knownType itemType;
            if (perceivedTypeCache.TryGetValue(extension, out itemType))
            {
                return itemType == knownType.Playlist;
            }
            return false;
        }

        public static bool IsEntity(string fileName)
        {
            if (Directory.Exists(fileName)) { return true; }

            string extension = Path.GetExtension(fileName).ToLower();
            if (String.IsNullOrEmpty(extension)) { return false; }

            if (perceivedTypeCache == null) 
            {
                perceivedTypeCache = getKnownTypes();
            }

            knownType itemType;
            if (perceivedTypeCache.TryGetValue(extension, out itemType))
            {
                return itemType != knownType.Other;
            }
            knownType type = determineType(extension);
            return type != knownType.Other;
        }

        public static knownType determineType(string extension)
        {
            string pt = null;
            RegistryKey key = Registry.ClassesRoot;
            key = key.OpenSubKey(extension);
            if (key != null)
            {
                pt = key.GetValue("PerceivedType") as string;
            }
            if (pt == null) pt = "";
            pt = pt.ToLower();

            lock (perceivedTypeCache)
            {
                if (pt == "audio")
                {
                    perceivedTypeCache.Add(extension, knownType.Song);
                }
                else
                {
                    perceivedTypeCache.Add(extension, knownType.Other);
                }
            }
            return perceivedTypeCache[extension];
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

            extentions = Config.GetInstance().GetListSetting("Extensions.Image");
            foreach (string extention in extentions)
            {
                retVal.Add(extention, knownType.Other);
            }

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

        static public string ImageCacheFullName(string key, string imageType)
        {
            string path = Config.GetInstance().GetSetting("CachePath") + "\\Images\\" + imageType + "\\" + key.Substring(0, 2);
            Directory.CreateDirectory(path);
            return path + "\\" + key + ".jpg";
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
            string[] pairs1 = new string[string1.Length - 1];
            for (int i = 0; i < string1.Length - 1; i++) { pairs1[i] = string1.Substring(i, 2); }

            if (pairs1.Count() == 0) { return 0; }

            string[] pairs2 = new string[string2.Length - 1];
            for (int i = 0; i < string2.Length - 1; i++) { pairs2[i] = string2.Substring(i, 2); }

            int hits = pairs1.Count(s1 => pairs2.Any(s2 => s1 == s2));

            return (100 * (hits / pairs1.Count()));
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
