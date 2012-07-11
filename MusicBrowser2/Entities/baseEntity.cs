using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using MusicBrowser.Engines.PlayState;
using MusicBrowser.Engines.ViewState;
using MusicBrowser.Models;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class baseEntity : BaseModel
    {
        #region variables
        private string _thumbPath;
        private string _bannerPath;
        private string _logoPath;
        private string _title;
        private List<string> _backgroundPaths;
        private int _duration;
        private string _overview;
        private DateTime _releaseDate;
        private int _rating;
        private Dictionary<String, DateTime> _metadata = new Dictionary<string, DateTime>();
        #endregion

        #region cached attributes
        [DataMember]
        public String Path { get; set; }
        [DataMember]
        public String ThumbPath
        {
            get
            {
                if (String.IsNullOrEmpty(_thumbPath))
                {
                    return DefaultThumbPath;
                }
                return _thumbPath;
            }
            set
            {
                _thumbPath = value;
                DataChanged("ThumbPath");
                DataChanged("Thumb");
            }
        }
        [DataMember]
        public String LogoPath
        {
            get
            {
                return _logoPath;
            }
            set
            {
                _logoPath = value;
                DataChanged("LogoPath");
                DataChanged("Logo");
                DataChanged("LogoExists");
            }
        }
        [DataMember]
        public String BannerPath
        {
            get
            {
                return _bannerPath;
            }
            set
            {
                _bannerPath = value;
                DataChanged("BannerPath");
                DataChanged("Banner");
                DataChanged("BannerExists");
            }
        }
        [DataMember]
        public virtual String Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                DataChanged("Title");
            }
        }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public DateTime LastUpdated { get; set; }
        [DataMember]
        public int Rating 
        {
            get
            {
                return _rating;
            }
            set
            {
                if (value != _rating)
                {
                    _rating = value;
                    DataChanged("Rating");
                }
            }
        }
        [DataMember]
        public bool Loved { get; set; }
        [DataMember]
        public List<String> BackgroundPaths 
        {
            get
            {
                return _backgroundPaths;
            }
            set
            {
                _backgroundPaths = value;
                DataChanged("BackgroundPaths");
            }
        }
        [DataMember]
        public int Duration
        {
            get { return _duration; }
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    DataChanged("Duration");
                }
            }
        }
        [DataMember]
        public string Overview
        {
            get { return _overview; }
            set
            {
                string cleansed = value.Replace("\r\n\r\n", "\r\n");
                if (cleansed != _overview)
                {
                    _overview = cleansed;
                    DataChanged("Overview");
                }
            }
        }
        [DataMember]
        public DateTime ReleaseDate
        {
            get 
            {
                if (_releaseDate.Year < 1500)
                {
                    return DateTime.MinValue;
                }
                return _releaseDate;
            }
            set
            {
                if (value != _releaseDate)
                {
                    _releaseDate = value;
                    DataChanged("ReleaseDate");
                }
            }
        }
        [DataMember]
        public Dictionary<String, DateTime> MetadataStamps 
        {
            get
            {
                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }
        #endregion

        #region non-cached attributes

        public bool Played
        {
            get
            {
                return (PlayState.Played);
            }
        }

        public new string Description
        {
            get
            {
                if (!String.IsNullOrEmpty(DefaultFormat))
                {
                    return TokenSubstitution(DefaultFormat);
                }
                return Title;
            }
        }

        public string CacheKey
        {
            get
            {
                if (String.IsNullOrEmpty(Path))
                {
                    throw new Exception("Entity Path needs to be set before a Cachekey can be created");
                }
                return Helper.GetCacheKey(Path);
            }
        }

        public Microsoft.MediaCenter.UI.Image Thumb
        {
            get
            {
                return Helper.GetImage(ThumbPath);
            }
        }

        public Microsoft.MediaCenter.UI.Image Banner
        {
            get
            {
                return Helper.GetImage(BannerPath);
            }
        }

        public Microsoft.MediaCenter.UI.Image Logo
        {
            get
            {
                return Helper.GetImage(LogoPath);
            }
        }

        public bool BannerExists
        {
            get
            {
                return System.IO.File.Exists(BannerPath);
            }
        }

        public bool LogoExists
        {
            get
            {
                return System.IO.File.Exists(LogoPath);
            }
        }

        public string SortName { get; set; }

        public string Kind
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual List<string> SortFields
        {
            get
            {
                return new List<string> { "Title", "Filename" };
            }
        }

        #endregion

        #region abstract methods
        public abstract string Serialize();
        public abstract void Play(bool queue, bool shuffle);
        public virtual string Information { get { return Kind; } }
        public abstract bool Playable { get; }
        #endregion

        #region abstract attributes
        public abstract IPlayState PlayState { get; }
        public abstract IViewState ViewState { get; }
        public abstract string DefaultThumbPath { get; }
        public virtual List<Microsoft.MediaCenter.UI.Image> CodecIcons { get { return new List<Microsoft.MediaCenter.UI.Image>(); } }
        protected virtual string DefaultFormat 
        {
            get
            {
                foreach (string kind in this.Tree())
                {
                    string key = String.Format("Entity.{0}.Format", kind);
                    if (!String.IsNullOrEmpty(Config.GetSetting(key)))
                    {
                        return Config.GetStringSetting(key);
                    }
                }
                return "[Title]";
            }
        }
        #endregion

        #region private helpers
        public virtual string TokenSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "title":
                    case "Title":
                        output = output.Replace("[" + token + "]", Title); break;
                    case "title:sort":
                    case "Title:sort":
                        output = output.Replace("[" + token + "]", HandleIgnoreWords(Title)); break;
                    case "Kind":
                    case "kind":
                    case "Kind:sort":
                    case "kind:sort": 
                        output = output.Replace("[" + token + "]", Kind); break;
                    case "filename":
                    case "Filename":
                    case "filename:sort":
                    case "Filename:sort":
                        string filename = string.Empty;
                        if (System.IO.File.Exists(Path))
                        {
                            filename = System.IO.Path.GetFileNameWithoutExtension(Path);
                        }
                        else if (System.IO.Directory.Exists(Path))
                        {
                            filename = System.IO.Path.GetFileName(Path);
                        }
                        output = output.Replace("[" + token + "]", filename);
                        break;
                    case "Added:sort":
                    case "added:sort":
                        if (TimeStamp > DateTime.Parse("01-JAN-1000")) 
                        {
                            output = output.Replace("[" + token + "]", TimeStamp.ToString("yyyyMMdd hhmmss")); 
                            break; 
                        }
                        output = output.Replace("[" + token + "]", ""); 
                        break;
                    case "Added":
                    case "added":
                        if (TimeStamp > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", TimeStamp.ToString("dd mmm yyyy"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "Duration":
                    case "duration":
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
                        output = output.Replace("[" + token + "]", length);
                        break;
                    case "Duration()":
                    case "duration()":
                        if (Duration <= 1)
                        {
                            output = output.Replace("[" + token + "]", "");
                            break;
                        }

                        TimeSpan td = TimeSpan.FromSeconds(Duration);
                        string lengthd;
                        if (td.Hours == 0)
                        {
                            lengthd = string.Format("({0}:{1:D2})", (Int32)Math.Floor(td.TotalMinutes), td.Seconds);
                        }
                        else
                        {
                            lengthd = string.Format("({0}:{1:D2}:{2:D2})", (Int32)Math.Floor(td.TotalHours), td.Minutes, td.Seconds);
                        }
                        output = output.Replace("[" + token + "]", lengthd);
                        break;
                    case "Duration:sort":
                    case "duration:sort":
                        output = output.Replace("[" + token + "]", Duration.ToString("D10")); break;
                    case "ReleaseDate":
                    case "releasedate":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("dd MMMM yyyy"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "ReleaseDate:sort":
                    case "releaserate:sort":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "ReleaseYear":
                    case "releaseyear":
                    case "ReleaseYear:sort":
                    case "releaseyear:sort":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.Year.ToString());
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "ReleaseYear()":
                    case "releaseyear()":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", "(" + ReleaseDate.Year + ")");
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "timesplayed":
                    case "TimesPlayed":
                        output = output.Replace("[" + token + "]", PlayState.TimesPlayed.ToString()); break;
                    case "timesplayed:sort":
                    case "TimesPlayed:sort":
                        output = output.Replace("[" + token + "]", PlayState.TimesPlayed.ToString("D6")); break;
                    case "LastPlayed:sort":
                    case "lastplayed:sort":
                        if (PlayState.LastPlayed > DateTime.Parse("01-JAN-1000")) 
                        {
                            output = output.Replace("[" + token + "]", PlayState.LastPlayed.ToString("yyyy-MM-dd HH:mm:ss:fff")); 
                            break; 
                        }
                        output = output.Replace("[" + token + "]", ""); 
                        break;
                    case "LastPlayed":
                    case "lastplayed":
                        if (PlayState.LastPlayed > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", PlayState.LastPlayed.ToString("dd MMMM yyyy"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                }
            }
            return output.Trim();
        }
        #endregion

        #region protected helpers

        private static readonly IEnumerable<string> SortIgnore = Config.GetListSetting("SortReplaceWords");
        protected static string HandleIgnoreWords(string value)
        {
            foreach (string item in SortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1); }
            }
            return value;
        }

        protected void DataChanged(string property)
        {
            FirePropertyChanged(property);
            FirePropertyChanged("Description");
            FirePropertyChanged("Information");
            FirePropertyChanged("CodecIcons");
            if (OnPropertyChanged != null)
            {
                OnPropertyChanged(property);
            }
        }

        public delegate void ChangedPropertyHandler(string property);
        public event ChangedPropertyHandler OnPropertyChanged;

        #endregion
    }
}