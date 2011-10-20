﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MusicBrowser.Entities;


namespace MusicBrowser.Util
{
    public class Config
    {
        private readonly XmlDocument _xml;
        private readonly string[,] _defaults = { 

                { "EnableFanArt", true.ToString() },
                { "UseFolderImageForTracks", true.ToString() },
                { "ShowVersion", false.ToString() },

                { "ShowClock", true.ToString() },
                { "WindowsLibrarySupport", true.ToString() },
                { "LogLevel", "error" },
                { "LogDestination", "file" },
                { "LogFile", Path.Combine(Helper.AppLogFolder, "MusicBrowser2.log") },
                { "ManualLibraryFile", Path.Combine(Helper.AppFolder, "MusicLibrary.vf") },
                { "CacheEngine", "FileSystem" },
                { "AutoLoadNowPlaying", false.ToString() },

                { "AutoPlaylistSize", "50" },

                { "ImagesByName", Path.Combine(Helper.AppFolder, "IBN") },
                { "UseInternetProviders", true.ToString() } ,
                { "LastFMUserName", String.Empty },

                { "SortReplaceWords", "the|a|an" },
                { "PutDiscInTrackNo", true.ToString() },
                                        
                { "Home.View", "Thumb" },
                { "Home.Format", "MusicBrowser 2" },
                { "Home.SortOrder", "[title]" },
                { "Home.SearchSummary", "[kind]" },
                { "Home.Summary", "" },

                { "Artist.View", "List" },
                { "Artist.Format", "[title]" },
                { "Artist.SortOrder", "[title]" },
                { "Artist.SearchSummary", "[kind]" },
                { "Artist.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Album.View", "List" },
                { "Album.Format", "([release]) [title]" },
                { "Album.SortOrder", "[release][title]" },
                { "Album.SearchSummary", "[kind],  [albumartist]" },
                { "Album.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Genre.View", "List" },
                { "Genre.Format", "[title]" },
                { "Genre.SortOrder", "[title]" },
                { "Genre.SearchSummary", "[kind] [title]" },
                { "Genre.Summary", "" },

                { "Track.View", "List" },
                { "Track.Format", "[track] - [title]" },
                { "Track.SortOrder", "[track][title]" },
                { "Track.SearchSummary", "[kind],  [artist],  [album]  ([codec])" },
                { "Track.Summary", "[playcount]  [listeners]  [allplays]" },

                { "Playlist.View", "" },
                { "Playlist.Format", "[title]" },
                { "Playlist.SortOrder", "" },
                { "Playlist.SearchSummary", "[kind]" },
                { "Playlist.Summary", "" },

                { "Folder.View", "list" },
                { "Folder.Format", "[title]" },
                { "Folder.SortOrder", "[title]" },
                { "Folder.SearchSummary", "[kind]" },
                { "Folder.Summary", "" },

                { "Group.View", "list" },
                { "Group.Format", "[title]" },
                { "Group.SortOrder", "[title]" },
                { "Group.SearchSummary", "[kind]" },
                { "Group.Summary", "" },

                { "Virtual.View", "list" },
                { "Virtual.Format", "[title]" },
                { "Virtual.SortOrder", "[title]" },
                { "Virtual.SearchSummary", "[kind]" },
                { "Virtual.Summary", "" },
                                        
                { "LogStatsOnClose", false.ToString() },

                { "Engine", "MediaCentre" },
                { "Player.foobar2000", (Is64Bit ? "C:\\Program Files (x86)" : "C:\\Program Files") + "\\foobar2000\\foobar2000.exe" },
                { "Player.VLC", (Is64Bit ? "C:\\Program Files (x86)" : "C:\\Program Files") + "\\VideoLAN\\VLC\\vlc.exe" },

                { "CachePath", Helper.CachePath },

                { "Extensions.Playlist", ".wpl|.m3u|.asx" },
                { "Extensions.Image", ".png|.jpg|.jpeg" },
                { "Extensions.Ignore", ".xml|.cue|.txt|.nfo" }
 
//                                        { "ShowCDs", true.ToString() }
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

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        readonly Dictionary<string, string> _settingCache = new Dictionary<string, string>();

        public void SetDefaultSettings()
        {
            for (int i = 0; i <= _defaults.GetUpperBound(1); i++)
            {
                GetSetting(_defaults[i,0]);
            }
        }

        private string GetSetting(string key)
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
            Logging.Logger.Debug(String.Format("Updating setting {0} to value {1}", key, value));

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
                    if (_xml.SelectSingleNode(string.Format("Settings/{0}", parts[0])) == null)
                    {
                        XmlNode pNode = _xml.CreateNode(XmlNodeType.Element, parts[0], string.Empty);
                        _xml.FirstChild.AppendChild(pNode);
                    }
                    else
                    {
                        XmlNode pNode = _xml.SelectSingleNode(string.Format("Settings/{0}", parts[0]));

                        node = _xml.CreateNode(XmlNodeType.Element, parts[1], string.Empty);
                        node.InnerText = value;
                        pNode.AppendChild(node);
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
            if (_sortIgnore == null) { _sortIgnore = _instance.GetListSetting("SortReplaceWords"); }

            foreach (string item in _sortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1); }
            }
            return value;
        }
    }
}
