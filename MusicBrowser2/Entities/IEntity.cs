using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Models;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    [DataContract]
    public enum EntityKind
    {
        Album,
        Artist,
        Folder,
        Home,
        Playlist,
        Song,
        Genre
    }

    [DataContract]
    public abstract class IEntity : BaseModel
    {
        private string _sortName;
        private string _cacheKey;

        public IEntity()
        {
            Performers = new List<string>();
            ProviderTimeStamps = new Dictionary<string, DateTime>();
            Genres = new List<string>();
        }

        [DataMember]
        public virtual string Path { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public string IconPath { get; set; }
        [DataMember]
        public string BackgroundPath { get; set; }
        [DataMember]
        public string MusicBrainzID { get; set; }
        [DataMember]
        public string DiscId { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string TrackName { get; set; }
        [DataMember]
        public string ArtistName { get; set; }
        [DataMember]
        public string AlbumArtist { get; set; }
        [DataMember]
        public string AlbumName { get; set; }
        [DataMember]
        public int TrackNumber { get; set; }
        [DataMember]
        public int DiscNumber { get; set; }
        [DataMember]
        public DateTime ReleaseDate { get; set; }
        [DataMember]
        public string Codec { get; set; }
        [DataMember]
        public string Channels { get; set; }
        [DataMember]
        public string SampleRate { get; set; }
        [DataMember]
        public string Resolution { get; set; }
        [DataMember]
        public int PlayCount { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public int Listeners { get; set; }
        [DataMember]
        public int TotalPlays { get; set; }
        [DataMember]
        public bool Favorite { get; set; }
        [DataMember]
        public List<string> Performers { get; set; }
        [DataMember]
        public List<string> Genres { get; set; }
        [DataMember]
        public string Lyrics { get; set; }
        [DataMember]
        public Dictionary<string, DateTime> ProviderTimeStamps { get; set; }

        // read only and have default values
        public virtual EntityKind Kind { get { return EntityKind.Home; } }
        public string KindName { get { return Kind.ToString(); } }
        public virtual string DefaultBackgroundPath { get { return string.Empty; } }
        public virtual string DefaultIconPath { get { return string.Empty; } }

        // Calculated/transient
        public int Index { get; set; }
        public virtual string ShortSummaryLine1 { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public int AlbumCount { get; set; }
        [DataMember]
        public int ArtistCount { get; set; }
        [DataMember]
        public int TrackCount { get; set; }

        // Read Only values
        public string SortName { get { return _sortName; } }
        public virtual string View { get { return Config.GetInstance().GetSetting(KindName + ".View").ToLower(); } }
        public virtual bool Playable { get { return false; } }

        // Fully implemented
        public Image Background
        {
            get
            {
                // backgrounds are disabled when fan art is disabled
                if (!Config.GetInstance().GetBooleanSetting("EnableFanArt"))
                {
                    return GetImage(String.Empty);
                }
                if (!String.IsNullOrEmpty(BackgroundPath))
                {
                    return GetImage(BackgroundPath);
                }
                if (!String.IsNullOrEmpty(DefaultBackgroundPath))
                {
                    return GetImage(DefaultBackgroundPath);
                }
                return GetImage(String.Empty);
            }
        }
        public Image Icon
        {
            get
            {
                // implementing with a "DefaultIconPath" allows the true resx path of the default icon
                // to be hidden from things like the cache
                if (String.IsNullOrEmpty(IconPath) || !System.IO.File.Exists(IconPath)) 
                {
                    return GetImage(DefaultIconPath); 
                }
                return GetImage(IconPath);
            }
        }
        public string CacheKey
        {
            get
            {
                if (String.IsNullOrEmpty(_cacheKey))
                {
                    _cacheKey = Helper.GetCacheKey(Path);
                }
                return _cacheKey;
            }
        }

        public virtual void UpdateValues()
        {
            // cache the sortname, we don't want to do this complex calc every time its accessed
            string sortName = MacroSubstitution(Config.GetInstance().GetSetting(KindName + ".SortOrder")).ToLower();
            sortName = Config.HandleIgnoreWords(sortName);
            Regex rgx = new Regex(@"[^#a-z0-9]+");
            _sortName = rgx.Replace(sortName, " ").Trim();

            FirePropertyChanged("ShortSummaryLine1");
            FirePropertyChanged("ShortSummaryLine2");
            FirePropertyChanged("OptionalArtistLine");
            FirePropertyChanged("Description");
            FirePropertyChanged("Background");
            FirePropertyChanged("Icon");
            FirePropertyChanged("Title");
        }

        private static Image GetImage(string path)
        {
            if (path.StartsWith("resx://"))
            {
                return new Image(path);
            }
            if (path.StartsWith("http://"))
            {
                return new Image(path);
            }
            if (System.IO.File.Exists(path))
            {
                return new Image("file://" + path);
            }
            return new Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
        }

        public string ShortSummaryLine2
        {
            get { return MacroSubstitution(Config.GetInstance().GetSetting(KindName + ".Summary")); }
        }

        public new string Description
        {
            get { return MacroSubstitution(Config.GetInstance().GetSetting(KindName + ".Format")); }
        }

        public string OptionalArtistLine
        {
            get
            {
                if (ArtistName != AlbumArtist && !String.IsNullOrEmpty(ArtistName) && !String.IsNullOrEmpty(AlbumArtist))
                {
                    return ArtistName;
                }
                return string.Empty;
            }
        }

        public string MacroSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "title":
                        output = output.Replace("[title]", Title); break;
                    case "description":
                        output = output.Replace("[description]", Description); break;
                    case "track":
                        if (Config.GetInstance().GetBooleanSetting("PutDiscInTrackNo"))
                        {
                            if (DiscNumber > 0)
                            {
                                output = output.Replace("[track]", DiscNumber.ToString() + "." + TrackNumber.ToString("D2")); break;
                            }
                        }
                        output = output.Replace("[track]", TrackNumber.ToString("D2")); break; 
                    case "trackname":
                        output = output.Replace("[trackname]", TrackName); break;
                    case "artist":
                        output = output.Replace("[artist]", ArtistName); break;
                    case "albumartist":
                        output = output.Replace("[albumartist]", AlbumArtist); break;
                    case "album":
                        output = output.Replace("[album]", AlbumName); break;
                    case "release.date":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { output = output.Replace("[release.date]", ReleaseDate.ToString("dd mmmm yyyy")); break; }
                        output = output.Replace("[release.date]", ""); break;
                    case "release":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000")) { output = output.Replace("[release]", ReleaseDate.ToString("yyyy")); break; }
                        output = output.Replace("[release]", ""); break;
                    case "channels":
                        output = output.Replace("[channels]", Channels); break;
                    case "samplerate":
                        output = output.Replace("[samplerate]", SampleRate); break;
                    case "resolution":
                        output = output.Replace("[resolution]", Resolution); break;
                    case "codec":
                        output = output.Replace("[codec]", Codec); break;
                    case "label":
                        output = output.Replace("[label]", Label); break;
                    case "playcount":
                        {
                            if (PlayCount > 0)
                            {
                                output = output.Replace("[playcount]", "Plays: " + PlayCount.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[playcount]", String.Empty);
                            break;
                        }
                    case "listeners":
                        {
                            if (Listeners > 0)
                            {
                                output = output.Replace("[listeners]", "Listeners: " + Listeners.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[listeners]", String.Empty);
                            break;
                        }
                    case "allplays":
                        {
                            if (TotalPlays > 0)
                            {
                                output = output.Replace("[allplays]", "Total Plays: " + TotalPlays.ToString("N0"));
                                break;
                            }
                            output = output.Replace("[allplays]", String.Empty);
                            break;
                        }
                    case "length":
                        TimeSpan t = TimeSpan.FromSeconds(Duration);
                        string length;
                        if (t.Hours == 0)
                        {
                            length = string.Format("{0}:{1:D2}", (Int32)Math.Floor(t.TotalMinutes), t.Seconds);
                        }
                        else
                        {
                            length = string.Format("{0}:{1:D2}:{2:D2}", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds);
                        }
                        output = output.Replace("[length]", length);
                        break;
                    case "kind":
                        output = output.Replace("[kind]", KindName); break;
                    case "tracks":
                        output = output.Replace("[tracks]", TrackCount.ToString("N0")); break;
                    case "rating":
                        output = output.Replace("[rating]", Rating.ToString()); break;
                    case "favorite":
                        if (Favorite) { output = output.Replace("[favorite]", "favorite"); break; }
                        output = output.Replace("[favorite]", ""); break;
                }

            }
            return output.Trim();
        }

    }
}
