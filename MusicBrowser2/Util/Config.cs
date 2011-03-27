using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities.Interfaces;
using System.Text.RegularExpressions;

namespace MusicBrowser.Util
{
    public class Config
    {
        private readonly XmlDocument _xml;
        private readonly string[,] _defaults = { 
                                      //  { "Language", "English" }, 
                                        { "EnableFanArt", true.ToString() },
                                        { "UseFolderImageForTracks", true.ToString() },

                                        { "ShowClock", true.ToString() },
                                        { "WindowsLibrarySupport", true.ToString() },
                                        { "LogLevel", "error" },
                                        { "LogDestination", "file" },
//                                        { "EnableHTTPLogging", false.ToString() },
                                        { "LogFile", Path.Combine(Helper.AppLogFolder, "MusicBrowser2.log") },
                                        { "ManualLibraryFile", Path.Combine(Helper.AppFolder, "MusicLibrary.vf") },
                                        { "EnableCache", true.ToString() },
                                        { "AutoLoadNowPlaying", false.ToString() },
                                        { "HomeBackground", Path.Combine(Helper.AppFolder, "backdrop.jpg") },

                                        { "IgnoreStartingThe", true.ToString() },
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
                                        { "FormatForAlbum", "[title]" },
                                        { "FormatForArtist", "[title]" },
                                        { "FormatForPlaylist", "[title]" },

                                        { "ThreadPoolSize", "2" }
                                      };

        #region singleton
        static Config _instance;
        static readonly object Padlock = new object();

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
                catch { }
            }
        }

        public static Config getInstance()
        {
            if (_instance != null) return _instance;
            _instance = new Config();
            return _instance;
        }
        #endregion

        readonly Dictionary<string, string> _settingCache = new Dictionary<string, string>();

        public string getSetting(string key)
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
                        setSetting(key, retval);
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

        public bool getBooleanSetting(string key)
        {
            return (getSetting(key).ToLower() == "true");
        }

        public void setSetting(string key, string value)
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

        public void resetSettings()
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
        public static string handleStartingThe(string value)
        {
            if (value.ToLower().StartsWith("the "))
            {
                if (_instance.getBooleanSetting("IgnoreStartingThe"))
                {
                    return value.Substring(4);
                }
            }
            return value;
        }

        public static string handleEntityView(EntityKind kind)
        {
            switch (kind)
            {
                case EntityKind.Home:
                    {
                        return _instance.getSetting("ViewForHome");
                    }
                case EntityKind.Artist:
                    {
                        return _instance.getSetting("ViewForArtist");
                    }
                case EntityKind.Album:
                    {
                        return _instance.getSetting("ViewForAlbum");
                    }
            }
            return _instance.getSetting("ViewForUnknown");
        }

        public static string handleEntityDescription(IEntity entity)
        {
            string format;
            switch (entity.Kind)
            {
                case EntityKind.NotDetermined: return entity.Title;
                case EntityKind.Unknown: return entity.Title;
                case EntityKind.Home: return entity.Title;
                case EntityKind.Song:
                    {
                        format = _instance.getSetting("FormatForSong");
                        break;
                    }
                case EntityKind.Album:
                    {
                        format = _instance.getSetting("FormatForAlbum");
                        break;
                    }
                case EntityKind.Artist:
                    {
                        format = _instance.getSetting("FormatForArtist");
                        break;
                    }
                case EntityKind.Playlist:
                    {
                        format = _instance.getSetting("FormatForPlaylist");
                        break;
                    }
                default:
                    {
                        format = _instance.getSetting("FormatForUnknown");
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
