using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Util
{
    public static class Config
    {
        private static readonly XmlDocument Xml = new XmlDocument();
        private static readonly bool Ready = GetReady();
        private static string ConfigFile;
        private static readonly Dictionary<string, string> SettingCache = new Dictionary<string, string>();
        private static readonly string[,] Defaults = { 

                { "LastRunVersion", "0.0.0.0" },

                { "EnableFanArt", true.ToString() },
                { "UseFolderImageForTracks", true.ToString() },
                { "ShowClock", true.ToString() },
                { "AutoLoadNowPlaying", false.ToString() },
                { "AutoPlaylistSize", "50" },
                { "SortReplaceWords", "the|a|an" },
                { "PutDiscInTrackNo", true.ToString() },
                { "ImagesByName", Path.Combine(AppFolder, "IBN") },
                { "PlayStateDatabase", Path.Combine(Path.Combine(AppFolder, "Cache"), "PlayState.db") },
                { "ViewStateDatabase", Path.Combine(Path.Combine(AppFolder, "Cache"), "ViewState.db") },

                { "PlaylistLimit", "1" },
                { "EnableMoviePlaylists", true.ToString() },

                { "ShowThumbs", true.ToString() },
                { "Language", "English" },

                { "KeyboardAction", "Jump" },

                { "Log.Level", "error" },
                { "Log.Destination", "file" },
                { "Log.File", Path.Combine(AppLogFolder, "MusicBrowser2.log") },

                { "Internet.UseProviders", true.ToString() } ,
                { "Internet.LastFMUserName", String.Empty },

                { "Collections.Folder",  Path.Combine(AppFolder, "Collections") },

                { "Cache.Path", Path.Combine(AppFolder, "Cache") },
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

        #region application folders

        public static string CachePath
        {
            get
            {
                string e = GetStringSetting("Cache.Path");
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

        private static string _appLogFolder;

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

        private static string _plugInFolder;

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

        private static string _componentFolder;

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

        private static string _appFolder;
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
        #endregion

        private static bool GetReady()
        {
            try
            {
                ConfigFile = Path.Combine(AppFolder, "MusicBrowser.config");
                Xml.Load(ConfigFile);
            }
            catch (Exception e) // there's been an error, delete the file and reset the config
            {
                try
                {
                    if (File.Exists(ConfigFile))
                    {
                        LoggerEngineFactory.Error(new Exception("Error reading config file, file is being reset, all settings will be lost.", e));
                        File.Delete(ConfigFile);
                    }
                    File.WriteAllText(ConfigFile, Resources.BlankSettings);
                    Xml.Load(ConfigFile);
                }
                catch (Exception) { }
            }

            return true;
        }

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        public static string GetSetting(string key)
        {
            // see if we've already cached the setting
            if (SettingCache.ContainsKey(key))
            {
                return SettingCache[key];
            }

            string retval = String.Empty;
            try
            {
                string xpathString = String.Format("Settings/{0}", key.Replace('.', '/'));
                XmlNode node = Xml.SelectSingleNode(xpathString);
                if (node != null)
                {
                    retval = Xml.SelectSingleNode(xpathString).InnerText;
                }
                else
                {
                    retval = String.Empty;
                }

                // if we've not got a value from the XML (usually the first run) get the default value
                if (String.IsNullOrEmpty(retval))
                {
                    bool found = false;
                    for (int x = 0; x < Defaults.GetLength(0); x++)
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
                    if (!found)
                    {
                        LoggerEngineFactory.Debug("Config", "No setting found for '" + key + "'");
                    }
                }
            }
            catch (Exception e)
            {
                retval = String.Empty;
            }
            // cache the setting on read
            SettingCache[key] = retval;
            return retval;
        }

        public static bool GetBooleanSetting(string key)
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

        public static int GetIntSetting(string key)
        {
            int value;
            if (Int32.TryParse(GetSetting(key), out value)) { return value; }
            return 0;
        }

        public static string GetStringSetting(string key)
        {
            return GetSetting(key);
        }

        public static IEnumerable<string> GetListSetting(string key)
        {
            return GetSetting(key).ToLower().Split('|');
        }

        public static void SetSetting(string key, string value)
        {
            string xpathString = String.Format("Settings/{0}", key.Replace('.', '/'));

            // update the cache
            SettingCache[key] = value;
            XmlNode node = Xml.SelectSingleNode(xpathString);
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
                        if (Xml.SelectSingleNode(path) == null)
                        {
                            XmlNode newNode = Xml.CreateNode(XmlNodeType.Element, parts[i], String.Empty);
                            // if this is the last item, save the "value"
                            if (i == (parts.Length - 1))
                            {
                                newNode.InnerText = value;
                            }
                            Xml.SelectSingleNode(parent).AppendChild(newNode);
                        }
                    }
                }
                else
                {
                    node = Xml.CreateNode(XmlNodeType.Element, key, String.Empty);
                    node.InnerText = value;
                    Xml.FirstChild.AppendChild(node);
                }
            }
            else
            {
                node.InnerText = value;
            }
            Xml.Save(ConfigFile);

            if (OnSettingUpdate != null)
            {
                OnSettingUpdate(key);
            }
        }

        public delegate void SettingsChangedHandler(String key);
        public static event SettingsChangedHandler OnSettingUpdate;
    }
}
