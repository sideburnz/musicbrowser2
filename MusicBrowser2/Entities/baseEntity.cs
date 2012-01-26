using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Util;
using System.Text.RegularExpressions;
using MusicBrowser.Models;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Providers;
using System.Drawing;
using ServiceStack.Text;

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
        private string _sortField;
        private int _thumbSize;
        private List<string> _backgroundPaths;
        private DateTime _lastPlayed;
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
        public virtual String View
        {
            get
            {
                // if view is overriden for this single entity, use its setting
                if (!String.IsNullOrEmpty(_view))
                {
                    return _view;
                }
                if (!String.IsNullOrEmpty(DefaultView))
                {
                    return DefaultView;
                }
                return "List";
            }
            set
            {
                _view = value;
                DataChanged("View");
            }
        }
        [DataMember]
        public virtual String SortField
        {
            get
            {
                if (!String.IsNullOrEmpty(_sortField))
                {
                    return _sortField;
                }
                if (!String.IsNullOrEmpty(DefaultSort))
                {
                    return DefaultSort;
                }
                return "[Title:sort]";
            }
            set
            {
                _sortField = value;
                DataChanged("SortField");
            }
        }
        [DataMember]
        public DateTime LastPlayed 
        {
            get
            {
                return _lastPlayed;
            }
            set
            {
                if (value != _lastPlayed)
                {
                    _lastPlayed = value;
                    DataChanged("LastPlayed");
                    DataChanged("Played");
                }
            }
        }
        [DataMember]
        public DateTime FirstPlayed { get; set; }
        [DataMember]
        public int TimesPlayed { get; set; }
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
        public int ThumbSize
        {
            get
            {
                if (_thumbSize == 0)
                {
                    return Config.GetInstance().GetIntSetting("Views.ThumbSize");
                }
                return _thumbSize;
            }
            set
            {
                if (_thumbSize != value)
                {
                    _thumbSize = value;
                    DataChanged("ThumbSize");
                }
            }
        }
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
            get { return _releaseDate; }
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

        #region private cached items
        [DataMember]
        private String _view = String.Empty;
        #endregion

        #region non-cached attributes
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

        public virtual string CacheKey
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
                return this.GetType().Name;
            }
        }

        public bool Played { get { return LastPlayed > DateTime.Parse("1000-01-01"); } }

        public Microsoft.MediaCenter.UI.Color Color
        {
            /// <summary>
            /// Provides a MediaCenter compatible "average" color for the image
            /// </summary>
            get
            {
                if (System.IO.File.Exists(ThumbPath))
                {
                    Bitmap image = new Bitmap(ThumbPath);
                    System.Drawing.Color baseColor = ImageProvider.CalculateAverageColor(image);
                    if (baseColor != null)
                    {
                        return new Microsoft.MediaCenter.UI.Color(baseColor.R, baseColor.G, baseColor.B);
                    }
                }
                return new Microsoft.MediaCenter.UI.Color(128, 128, 128);
            }
        }
        #endregion

        #region abstract methods
        public abstract string Serialize();
        public abstract void Play(bool queue, bool shuffle);
        public virtual string Information { get { return Kind; } }
        #endregion

        #region abstract attributes
        public abstract string DefaultThumbPath { get; }
        public abstract string DefaultFormat { get; }
        public abstract string DefaultSort { get; }
        public abstract string DefaultView { get; }
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
                            output = output.Replace("[" + token + "]", TimeStamp.ToString("yyyymmdd hhMMss")); 
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
                    case "Duration:sort":
                    case "duration:sort":
                        output = output.Replace("[" + token + "]", Duration.ToString("D10")); break;
                    case "ReleaseDate":
                    case "releasedate":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("dd mmm yyyy"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "ReleaseDate:sort":
                    case "releaserate:sort":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("yyyymmdd hhMMss"));
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

                }
            }
            return output.Trim();
        }
        #endregion

        #region protected helpers

        private static IEnumerable<string> _sortIgnore = Util.Config.GetInstance().GetListSetting("SortReplaceWords");
        protected static string HandleIgnoreWords(string value)
        {
            foreach (string item in _sortIgnore)
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
            if (!(OnPropertyChanged == null))
            {
                OnPropertyChanged(property);
            }
        }

        public delegate void ChangedPropertyHandler(string property);
        public event ChangedPropertyHandler OnPropertyChanged;

        #endregion
    }

    public static class Extensions
    {
        // so we can inherit values, we need someway of working out the object
        // heirarchy, this returns the types back to the baseEntity
        public static IEnumerable<string> Tree(this baseEntity e)
        {
            Type node = e.GetType();
            List<String> ret = new List<String>();

            while (node != typeof(BaseModel))
            {
                ret.Add(node.Name);
                node = node.BaseType;
            }
            return ret;
        }

        public static bool InheritsFrom<T>(this object e)
        {
            return typeof(T).IsAssignableFrom(e.GetType());
        }
    }
}