using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Win32;
using MusicBrowser.Providers;

namespace MusicBrowser.Util
{
    public static class Helper
    {
        private static readonly Random Random = new Random();

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
            get 
            {
                string e = Path.Combine(AppFolder, "Cache");
                if (!Directory.Exists(e))
                {
                    try
                    {
                        Directory.CreateDirectory(e);
                        Directory.CreateDirectory(e + "\\Images");
                        Directory.CreateDirectory(e + "\\Images\\Backgrounds");
                        Directory.CreateDirectory(e + "\\Images\\Covers");
                        Directory.CreateDirectory(e + "\\Images\\Thumbs");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Cache folder for MusicBrowser is missing: " + e, ex);
                    }
                }
                return e; 
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

        static string _componentFolder;
        public static string ComponentFolder
        {
            get
            {
                if (_componentFolder == null)
                {
                    var e = Path.Combine(AppFolder, "Components");
                    if (!Directory.Exists(e))
                    {
                        Directory.CreateDirectory(e);
                    }
                    _componentFolder = e;
                }
                return _componentFolder;
            }
        }

        #endregion

        #region file identifiers

        public enum KnownType
        {
            Track,
            Playlist,
            Folder,
            Video,
            Other
        }

        private static readonly Dictionary<string, KnownType> PerceivedTypeCache = GetKnownTypes();

        private static KnownType DetermineType(string extension)
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

                lock (PerceivedTypeCache)
                {
                    switch (pt)
                    {
                        case "video":
                            PerceivedTypeCache.Add(extension, KnownType.Video);
                            break;
                        case "audio":
                            PerceivedTypeCache.Add(extension, KnownType.Track);
                            break;
                        default:
                            PerceivedTypeCache.Add(extension, KnownType.Other);
                            break;
                    }
                }
            }
            catch
            {
                // if there's a problem, return an unknown type
                return KnownType.Other;
            }
            return PerceivedTypeCache[extension];
        }

        public static KnownType GetKnownType(FileSystemItem item)
        {
            // if it's a folder, don't worry about the type cache
            if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return KnownType.Folder;
            }

            string extension = Path.GetExtension(item.Name).ToLower();
            KnownType itemType;
            // try to get the item from the type cache
            if (PerceivedTypeCache.TryGetValue(extension, out itemType))
            {
                return itemType;
            }
            return DetermineType(extension);
        }

        private static Dictionary<string, KnownType> GetKnownTypes()
        {
            IEnumerable<string> extentions = Config.GetInstance().GetListSetting("Extensions.Playlist");
            Dictionary<string,KnownType> retVal = extentions.ToDictionary(extention => extention, extention => KnownType.Playlist);

            extentions = Config.GetInstance().GetListSetting("Extensions.Ignore");
            foreach (string extention in extentions)
            {
                retVal.Add(extention, KnownType.Other);
            }
            // special circumstance
            retVal.Add(String.Empty, KnownType.Other);

            return retVal;
        }

        #endregion

/*
        public static XmlNode CreateXmlNode(XmlDocument parent, string name, string value)
        {
            XmlNode node = parent.CreateNode(XmlNodeType.Element, name, "");
            node.InnerText = value;
            return node;
        }
*/

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

        // moved so it isn't instatiated for every call to the cache key code
        private static readonly SHA256CryptoServiceProvider Sha256Provider = new SHA256CryptoServiceProvider();

        static public string GetCacheKey(string seed)
        {
            // keys are 64 bytes
            byte[] buffer = Encoding.Unicode.GetBytes(seed.ToLowerInvariant());
            string hash = BitConverter.ToString(Sha256Provider.ComputeHash(buffer)).Replace("-", String.Empty);
            return hash;
        }

        static public string ImageCacheFullName(string key, ImageType type, int index)
        {
            string path = Config.GetInstance().GetStringSetting("Cache.Path") + "\\Images\\" + type.ToString() + "\\" + key.Substring(0, 2);
            Directory.CreateDirectory(path);
            if (index < 0)
            {
                return path + "\\" + key + ".png";
            }
            return path + "\\" + key + " - " + index + ".png";
        }

        public static string IBNPath(string category, string title)
        {
            return Path.Combine(Path.Combine(Config.GetInstance().GetStringSetting("ImagesByName"), category), title);
 
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

        private static string Cleanse(string raw)
        {
            string a = raw.ToLower();
            if (a.StartsWith("the ")) { return a.Substring(4).Trim(); }
            return a;
        }

        public static bool IsDVD (string path)
        {
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(path);
            return items.Any(item => item.Name.ToLower() == "video_ts");
        }

        public static Microsoft.MediaCenter.UI.Image GetImage(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return new Microsoft.MediaCenter.UI.Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
            }
            if (path.StartsWith("resx://"))
            {
                Microsoft.MediaCenter.UI.Image image = new Microsoft.MediaCenter.UI.Image(path);
                return image;
            }
            if (path.StartsWith("http://"))
            {
                return new Microsoft.MediaCenter.UI.Image(path);
            }
            if (File.Exists(path))
            {
                return new Microsoft.MediaCenter.UI.Image("file://" + path);
            }
            return new Microsoft.MediaCenter.UI.Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
        }

        public static IEnumerable<FileSystemItem> FilterInternalFiles(this IEnumerable<FileSystemItem> raw)
        {
            return raw
                .Where(item => !item.FullPath.Contains(@"\VIDEO_TS"))
                .Where(item => !item.FullPath.Contains(@"\AUDIO_TS"))
                .Where(item => !item.FullPath.Contains(@"\metadata\"));
        }

        //public static IList<TE> ShuffleList<TE>(this IList<TE> list)
        //{
        //    IList<TE> temp = list;
        //    if (temp.Count > 1)
        //    {
        //        for (int i = temp.Count - 1; i >= 0; i--)
        //        {
        //            TE tmp = temp[i];
        //            int randomIndex = Random.Next(i + 1);
        //            //Swap elements
        //            temp[i] = temp[randomIndex];
        //            temp[randomIndex] = tmp;
        //        }
        //    }
        //    return temp;
        //}

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(item => Random.Next());
        }

        private static readonly Regex EpisodeRegEx = new Regex(@"^[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<epnumber>\d{1,3})");

        public static bool IsEpisode(string path)
        {
            return EpisodeRegEx.Match(path).Success;
        }
    }
}
