using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Models;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    public enum EntityKind
    {
        Album,
        Artist,
        Disc,
        Folder,
        Home,
        Playlist,
        Song,
        Genre,
        Unknown
    }

    public class IEntity : BaseModel
    {
        private string _sortName;
        private readonly string _view;
        private string _cacheKey;
        private readonly string _summary2Format;

        public IEntity()
        {
            _view = Config.HandleEntityView(Kind).ToLower();
            _summary2Format = Config.GetInstance().GetSetting("SummaryLineFormatFor" + KindName);
            DefaultBackgroundPath = string.Empty;
            Performers = new List<string>();
            ProviderTimeStamps = new Dictionary<string, DateTime>();
            Genres = new List<string>();
            Dirty = false;
        }

        public virtual string Path { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string IconPath { get; set; }
        public string DefaultIconPath { get; set; }
        public string DefaultBackgroundPath { get; set; }
        public string BackgroundPath { get; set; }
        public string MusicBrainzID { get; set; }

        public string DiscId { get; set; }
        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumArtist { get; set; }
        public string AlbumName { get; set; }
        public int TrackNumber { get; set; }
        public int DiscNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Codec { get; set; }
        public string Channels { get; set; }
        public string SampleRate { get; set; }
        public string Resolution { get; set; }
        public int PlayCount { get; set; }
        public int Rating { get; set; }
        public int Listeners { get; set; }
        public int TotalPlays { get; set; }
        public bool Favorite { get; set; }
        public List<string> Performers { get; set; }
        public List<string> Genres { get; set; }
        public string Lyrics { get; set; }

        // read only and have default values
        public virtual EntityKind Kind { get { return EntityKind.Unknown; } }
        public string KindName { get { return Kind.ToString(); } }

        public Dictionary<string, DateTime> ProviderTimeStamps { get; set; }

        // Calculated/transient
        public int Index { get; set; }
        public virtual string ShortSummaryLine1 { get; set; }
        public int Duration { get; set; }
        public int Children { get; set; }
        public int TrackCount { get; set; }
        public bool Dirty { get; set; }
        public long Version { get; set; }
        public virtual IEntity Parent { get; set; }

        // Read Only values
        public string SortName { get { return _sortName; } }
        public virtual string View { get { return _view; } }
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
                if (Parent != null)
                {
                    if (!Parent.Kind.Equals(EntityKind.Home) && !Kind.Equals(EntityKind.Home))
                    {
                        return Parent.Background;
                    }
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
            _sortName = Config.HandleIgnoreWords(MacroSubstitution(Config.GetInstance().GetSetting("SortFor" + KindName))).ToLower();

            FirePropertyChanged("ShortSummaryLine1");
            FirePropertyChanged("ShortSummaryLine2");
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
            get { return MacroSubstitution(_summary2Format); }
        }

        public new string Description
        {
            get { return MacroSubstitution(Config.GetInstance().GetSetting("FormatFor" + KindName)); }
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
                        if (ReleaseDate > DateTime.MinValue) { output = output.Replace("[release.date]", ReleaseDate.ToString("dd mmmm yyyy")); break; }
                        output = output.Replace("[release.date]", ""); break;
                    case "release":
                        if (ReleaseDate > DateTime.MinValue) { output = output.Replace("[release]", ReleaseDate.ToString("yyyy")); break; }
                        output = output.Replace("[release]", "0000"); break;
                    case "channels":
                        output = output.Replace("[channels]", Channels); break;
                    case "samplerate":
                        output = output.Replace("[samplerate]", SampleRate); break;
                    case "resolution":
                        output = output.Replace("[resolution]", Resolution); break;
                    case "codec":
                        output = output.Replace("[codec]", Codec); break;
                    case "playcount":
                        output = output.Replace("[playcount]", PlayCount.ToString("N0")); break;
                    case "listeners":
                        output = output.Replace("[listeners]", Listeners.ToString("N0")); break;
                    case "allplays":
                        output = output.Replace("[allplays]", TotalPlays.ToString("N0")); break;
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
                    case "children":
                        output = output.Replace("[children]", Children.ToString("N0")); break;
                    case "rating":
                        output = output.Replace("[rating]", Rating.ToString()); break;
                    case "favorite":
                        if (Favorite) { output = output.Replace("[favorite]", "favorite"); break; }
                        output = output.Replace("[favorite]", ""); break;
                }

            }
            return output;
        }

    }
}
