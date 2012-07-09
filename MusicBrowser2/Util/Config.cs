using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Util
{
    public class Config
    {
        private readonly XmlDocument _xml;
        private readonly static object obj = new object();
        private static readonly string[,] Defaults = { 

                { "LastRunVersion", "0.0.0.0" },

                { "EnableFanArt", true.ToString() },
                { "UseFolderImageForTracks", true.ToString() },
                { "ShowClock", true.ToString() },
                { "AutoLoadNowPlaying", false.ToString() },
                { "AutoPlaylistSize", "50" },
                { "SortReplaceWords", "the|a|an" },
                { "PutDiscInTrackNo", true.ToString() },
                { "ImagesByName", Path.Combine(Helper.AppFolder, "IBN") },
                { "PlayStateDatabase", Path.Combine(Path.Combine(Helper.AppFolder, "Cache"), "PlayState.db") },
                { "ViewStateDatabase", Path.Combine(Path.Combine(Helper.AppFolder, "Cache"), "ViewState.db") },

                { "PlaylistLimit", "1" },
                { "EnableMoviePlaylists", true.ToString() },

                { "ShowThumbs", true.ToString() },
                { "Language", "English" },

                { "KeyboardAction", "Jump" },

                { "Log.Level", "error" },
                { "Log.Destination", "file" },
                { "Log.File", Path.Combine(Helper.AppLogFolder, "MusicBrowser2.log") },

                { "Internet.UseProviders", true.ToString() } ,
                { "Internet.LastFMUserName", String.Empty },

                { "Collections.Folder",  Path.Combine(Helper.AppFolder, "Collections") },

                { "Cache.Path", Path.Combine(Helper.AppFolder, "Cache") },
                { "Cache.Enable", true.ToString() },
                                        
                { "Telemetry.Participate", false.ToString() },
                { "Telemetry.ID", Guid.NewGuid().ToString() },

                { "Player.Engine", "MediaCentre" },
                { "Player.Paths.foobar2000", (Is64Bit ? "C:\\Program Files (x86)" : "C:\\Program Files") + "\\foobar2000\\foobar2000.exe" },
                { "Player.URLs.foobar2000", @"http://127.0.0.1:8888/musicbrowser2" },
                { "Player.DisableScreenSaver", true.ToString() },

                { "Extensions.Playlist", ".wpl|.m3u|.asx" },
                { "Extensions.Ignore", ".xml|.cue|.txt|.nfo" },
                { "Extensions.Image", ".png|.jpg|.jpeg" },
 
                { "Views.IsHorizontal", true.ToString() },
                { "Views.List.ShowSummary", true.ToString() },
                { "Views.Strip.ShowSummary", true.ToString() },
                { "Views.ThumbSize", "160" },

                { "ThemeLoader", "default" },

                { "Language", "" }

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
                        LoggerEngineFactory.Error(new Exception("Error reading config file, file is being reset, all settings will be lost.", e));
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
            lock (obj)
            {
                if (_instance == null)
                {
                    _instance = new Config();
                    return _instance;
                }
                return _instance;
            }
        }
        #endregion

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        readonly Dictionary<string, string> _settingCache = new Dictionary<string, string>();

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
                string xpathString = string.Format("Settings/{0}", key.Replace('.', '/'));
                retval = _xml.SelectSingleNode(xpathString).InnerText;
            }
            catch { } //BUG: this is rethrowing
            // if we've not got a value from the XML (usually the first run) get the default value
            if (String.IsNullOrEmpty(retval))
            {
                bool found = false;
                for (int x = 0; x < Defaults.GetLength(0); x++ )
                {
                    if (Defaults[x, 0] == key)
                    {
                        //save the value to the XML
                        retval = Defaults[x, 1];
                        SetSetting(key, retval);
                        found = true;
                        break;
                    }
                }
                if (!found) { LoggerEngineFactory.Debug("Config", "No setting found for '" + key + "'"); }
            }
            // cache the setting on read
            _settingCache[key] = retval;
            return retval;
        }

        public bool GetBooleanSetting(string key)
        {
            try
            {
                return (GetSetting(key).ToLower() == "true");
            }
            catch
            {
                return false;
            }
        }

        public int GetIntSetting(string key)
        {
            int value;
            if (int.TryParse(GetSetting(key), out value)) { return value; }
            return 0;
        }

        public string GetStringSetting(string key)
        {
            return GetSetting(key);
        }

        public IEnumerable<string> GetListSetting(string key)
        {
            return GetSetting(key).ToLower().Split('|');
        }

        public void SetSetting(string key, string value)
        {
            string configFile = Helper.AppConfigFile;
            string xpathString = string.Format("Settings/{0}", key.Replace('.', '/'));

            // update the cache
            _settingCache[key] = value;
            XmlNode node = _xml.SelectSingleNode(xpathString);
            if (node == null)
            {
                if (key.Contains("."))
                {
                    string[] parts = key.Split('.');
                    string path = "Settings";

                    for (int i = 0; i < parts.Length; i++)
                    {
                        string parent = path;
                        // we build the path as we go
                        path = path + "/" + parts[i];
                        // if the part of the path we're looking at doesn't exist, create it
                        if (_xml.SelectSingleNode(path) == null)
                        {
                            XmlNode newNode = _xml.CreateNode(XmlNodeType.Element, parts[i], string.Empty);
                            // if this is the last item, save the "value"
                            if (i == (parts.Length - 1))
                            {
                                newNode.InnerText = value;
                            }
                            _xml.SelectSingleNode(parent).AppendChild(newNode);
                        }
                    }
                }
                else
                {
                    node = _xml.CreateNode(XmlNodeType.Element, key, string.Empty);
                    node.InnerText = value;
                    _xml.FirstChild.AppendChild(node);
                }
            }
            else
            {
                node.InnerText = value;
            }
            _xml.Save(configFile);

            if (OnSettingUpdate != null)
            {
                OnSettingUpdate(key);
            }
        }

        public delegate void SettingsChangedHandler(String key);
        public static event SettingsChangedHandler OnSettingUpdate;
    }
}
