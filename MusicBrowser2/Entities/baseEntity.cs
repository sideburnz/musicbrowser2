﻿using System;
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

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class baseEntity : BaseModel
    {
        #region variables
        private string _thumbPath;
        private string _bannerPath;
        private string _title;
        private string _sortName;
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
                FirePropertiesChanged("ThumbPath", "Thumb");
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
                FirePropertiesChanged("BannerPath", "Banner", "BannerExists");
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
                FirePropertiesChanged("Title", "Description");
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
                FirePropertyChanged("View");
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
                        return MacroSubstitution(Config.GetInstance().GetStringSetting(setting));
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
                return GetImage(ThumbPath);
            }
        }

        public Microsoft.MediaCenter.UI.Image Banner
        {
            get
            {
                return GetImage(BannerPath);
            }
        }

        public bool BannerExists
        {
            get
            {
                return System.IO.File.Exists(BannerPath);
            }
        }

        public int Index { get; set; }

        public string SortName
        {
            get
            {
                return _sortName;
            }
            set
            {
                _sortName = HandleSortIgnoreWords(value);
            }
        }

        public string Kind
        {
            get
            {
                return this.GetType().Name;
            }
        }

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

        #region abstract attributes
        public abstract string DefaultThumbPath { get; }
        #endregion

        #region private helpers
        private string MacroSubstitution(string input)
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
                    case "kind":
                        output = output.Replace("[kind]", Kind); break;
                }
            }
            return output.Trim();
        }
        #endregion

        #region protected helpers
        protected static Microsoft.MediaCenter.UI.Image GetImage(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return new Microsoft.MediaCenter.UI.Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
            }
            if (path.StartsWith("resx://"))
            {
                return new Microsoft.MediaCenter.UI.Image(path);
            }
            if (path.StartsWith("http://"))
            {
                return new Microsoft.MediaCenter.UI.Image(path);
            }
            if (System.IO.File.Exists(path))
            {
                return new Microsoft.MediaCenter.UI.Image("file://" + path);
            }
            return new Microsoft.MediaCenter.UI.Image("resx://MusicBrowser/MusicBrowser.Resources/nullImage");
        }

        private static IEnumerable<string> _sortIgnore = Util.Config.GetInstance().GetListSetting("SortReplaceWords");
        protected static string HandleSortIgnoreWords(string value)
        {
            foreach (string item in _sortIgnore)
            {
                if (value.ToLower().StartsWith(item + " ")) { return value.Substring(item.Length + 1); }
            }
            return value;
        }
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