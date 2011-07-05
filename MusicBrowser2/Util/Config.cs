using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;


namespace MusicBrowser.Util
{
    public class Config
    {
        private readonly XmlDocument _xml;
        private readonly string[,] _defaults = { 
                                      //  { "Language", "English" }, 
                                        { "EnableFanArt", true.ToString() },
                                        { "UseFolderImageForTracks", true.ToString() },
                                        { "ShowVersion", false.ToString() },

                                        { "ShowClock", true.ToString() },
                                        { "WindowsLibrarySupport", true.ToString() },
                                        { "LogLevel", "error" },
                                        { "LogDestination", "file" },
                                        { "LogFile", Path.Combine(Helper.AppLogFolder, "MusicBrowser2.log") },
                                        { "ManualLibraryFile", Path.Combine(Helper.AppFolder, "MusicLibrary.vf") },
                                        { "EnableCache", true.ToString() },
                                        { "AutoLoadNowPlaying", false.ToString() },
                                        { "HomeBackground", Path.Combine(Helper.AppFolder, "backdrop.jpg") },

                                        { "SortReplaceWords", "the|a|an" },
                                        { "PutDiscInTrackNo", true.ToString() },

                                        { "LastFMUserName", string.Empty },
                                        { "UseInternetProviders", true.ToString() },
                                      //  { "LastFMPasswordHash", string.Empty },
                                      //  { "ScrobbleToFile", false.ToString() },
                                      //  { "EnableScrobbling", false.ToString() },
                                        
                                        { "ViewForHome", "Thumb" },
                                        { "ViewForArtist", "List" },
                                        { "ViewForAlbum", "List" },
                                        { "ViewForUnknown", "List" },

                                        { "FormatForUnknown", "[title]" },
                                        { "FormatForSong", "[track] - [title]" },
                                        { "FormatForAlbum", "([release]) [title]" },
                                        { "FormatForArtist", "[title]" },
                                        { "FormatForPlaylist", "[title]" },

                                        { "ThreadPoolSize", "2" },
                                        { "LogStatsOnClose", false.ToString() },

                                        { "Engine", "MediaCentre" },
                                        { "foobar2000", "C:\\Program Files\\foobar2000\\foobar2000.exe" },

                                        { "CachePath", Helper.CachePath },
 
                                        { "ShowCDs", true.ToString() }
                                      };

        #region singleton
        static Config _instance;

        Config()
        {
            string configFile = Helper.AppConfigFile;
            try
            {
                _xml = new XmlDocument();
                _xml.Load(configFile);
            }
            catch (Exception e) // there's been an error, delete the file and reset the config
            {
                try
                {
                    if (File.Exists(configFile))
                    {
                        Logging.Logger.Error(new Exception("Error reading config file, file is being reset, all settings will be lost.", e));
                        File.Delete(configFile);
                    }
                    File.WriteAllText(configFile, Resources.BlankSettings);
                    _xml = new XmlDocument();
                    _xml.Load(configFile);
                }
                catch (Exception) { }
            }
        }

        public static Config GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = new Config();
            return _instance;
        }
        #endregion

        readonly Dictionary<string, string> _settingCache = new Dictionary<string, string>();

        public void SetDefaultSettings()
        {
            for (int i = 0; i < _defaults.GetUpperBound(1); i++)
            {
                GetSetting(_defaults[i,0]);
            }
        }

        public string GetSetting(string key)
        {
            // see if we've already cached the setting
            if (_settingCache.ContainsKey(key))
            {
                return _settingCache[key];
            }

            string retval = string.Empty;
            try
            {
                string xpathString = string.Format("Settings/{0}", key);
                retval = _xml.SelectSingleNode(xpathString).InnerText;
            }
            catch { }
            // if we've not got a value from the XML (usually the first run) get the default value
            if (String.IsNullOrEmpty(retval))
            {
                bool found = false;
                for (int x = 0; x < _defaults.GetLength(0); x++ )
                {
                    if (_defaults[x, 0] == key)
                    {
                        //save the value to the XML
                        retval = _defaults[x, 1];
                        SetSetting(key, retval);
                        found = true;
                        break;
                    }
                }
                if (!found) { Logging.Logger.Error(new InvalidDataException("No setting found for '" + key + "'")); }
            }
            // cache the setting on read
            _settingCache[key] = retval;
            return retval;
        }

        public bool GetBooleanSetting(string key)
        {
            return (GetSetting(key).ToLower() == "true");
        }

        public void SetSetting(string key, string value)
        {
            string configFile = Helper.AppConfigFile;
            string xpathString = string.Format("Settings/{0}", key);

            // update the cache
            _settingCache[key] = value;
            XmlNode node = _xml.SelectSingleNode(xpathString);
            if (node == null)
            {
                node = _xml.CreateNode(XmlNodeType.Element, key, string.Empty);
                node.InnerText = value;
                _xml.FirstChild.AppendChild(node);
            }
            else
            {
                node.InnerText = value;
            }
            _xml.Save(configFile);
        }

        public void ResetSettings()
        {
            string configFile = Helper.AppConfigFile;
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
            // reset the cache too
            _settingCache.Clear();
        }

        // this pushes the intelligence involved with some settings to the config manager
        static IEnumerable<string> _sortIgnore;
        public static string HandleIgnoreWords(string value)
        {
            if (_sortIgnore == null) { _sortIgnore = _instance.GetSetting("SortReplaceWords").Split('|'); }

            foreach (string item in _sortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1); }
            }
            return value;
        }

        public static string HandleEntityView(EntityKind kind)
        {
            switch (kind)
            {
                case EntityKind.Home:
                    {
                        return _instance.GetSetting("ViewForHome");
                    }
                case EntityKind.Artist:
                    {
                        return _instance.GetSetting("ViewForArtist");
                    }
                case EntityKind.Album:
                    {
                        return _instance.GetSetting("ViewForAlbum");
                    }
            }
            return _instance.GetSetting("ViewForUnknown");
        }

        public static string HandleEntityDescription(IEntity entity)
        {
            string format;
            switch (entity.Kind)
            {
                case EntityKind.Disc:
                    {
                        return entity.Title;
                    }
                case EntityKind.Song:
                    {
                        format = _instance.GetSetting("FormatForSong");
                        break;
                    }
                case EntityKind.Album:
                    {
                        format = _instance.GetSetting("FormatForAlbum");
                        break;
                    }
                case EntityKind.Artist:
                    {
                        format = _instance.GetSetting("FormatForArtist");
                        break;
                    }
                case EntityKind.Playlist:
                    {
                        format = _instance.GetSetting("FormatForPlaylist");
                        break;
                    }
                case EntityKind.Unknown: return entity.Title;
                case EntityKind.Home: return entity.Title;
                default:
                    {
                        format = _instance.GetSetting("FormatForUnknown");
                        break;
                    }
            }

            // this swaps out the place holders with content from the dictionary
            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(format))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                if (token.Equals("title"))
                {
                    format = format.Replace("[title]", entity.Title);
                }
                else if (token.Equals("track") && entity.Kind.Equals(EntityKind.Song))
                {
                    format = format.Replace("[track]", ((Song)entity).Track);
                }
                else if (entity.Properties.ContainsKey(token))
                {
                    format = format.Replace("[" + token + "]", entity.Properties[token]);
                }
                else
                {
                    format = format.Replace("[" + token + "]", string.Empty);
                }
            }
            return format;
        }

    }
}
