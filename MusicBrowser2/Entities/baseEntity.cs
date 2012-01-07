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
        private string _title;
        private string _sortField;
        private int _thumbSize;
        private List<string> _backgroundPaths;
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
                DataChanged("Description");
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
                if (!string.IsNullOrEmpty(_view))
                {
                    return _view;
                }
                // if there's a view defined in the config, use it
                string setting = String.Format("Entity.{0}.View", Kind);
                if (Config.GetInstance().Exists(setting))
                {
                    return Config.GetInstance().GetStringSetting(setting);
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
                string setting = String.Format("Entity.{0}.SortBy", Kind);
                if (Config.GetInstance().Exists(setting))
                {
                    return Config.GetInstance().GetStringSetting(setting);
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
        public DateTime LastPlayed { get; set; }
        [DataMember]
        public int TimesPlayed { get; set; }
        [DataMember]
        public int Rating { get; set; }
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
        #endregion

        #region private cached items
        [DataMember]
        private String _view = String.Empty;
        #endregion

        public new string Description
        {
            get
            {
                IEnumerable<String> tree = this.Tree();
                foreach (string leaf in tree)
                {
                    string setting = String.Format("Entity.{0}.DisplayFormat", leaf);

                    if (Config.GetInstance().Exists(setting))
                    {
                        return TokenSubstitution(Config.GetInstance().GetStringSetting(setting));
                    }
                }
                // if we can't find anything, just return a fixed result
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

        public bool BannerExists
        {
            get
            {
                return System.IO.File.Exists(BannerPath);
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

        public bool Played { get { return LastPlayed > DateTime.Parse("2000-01-01"); } }

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

        public abstract string Serialize();

        public abstract void Play(bool queue, bool shuffle);

        #region abstract attributes
        public abstract string DefaultThumbPath { get; }
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
                            output = output.Replace("[" + token + "]", TimeStamp.ToString("yyyy-mm-dd hh:MM:ss")); 
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
    }
}