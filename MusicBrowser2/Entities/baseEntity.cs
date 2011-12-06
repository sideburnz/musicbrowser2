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

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class baseEntity : BaseModel
    {
        #region variables
        private string _thumbPath;
        private string _title;
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
        public DateTime CreateDate { get; set; }
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
                this.UpdateCache();
            }
        }
        #endregion

        #region private cached items
        [DataMember]
        private String _view = String.Empty;
        #endregion

        /********************
         * values needed for displaying
         *  - SummaryLine
         *  - Summary
         *  - SummaryLine2
         */

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

        public Image Thumb
        {
            get
            {
                return GetImage(ThumbPath);
            }
        }

        public int Index { get; set; }

        public string Kind
        {
            get
            {
                return this.GetType().Name;
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
        protected static Image GetImage(string path)
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