using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Util;
using MusicBrowser.Models;

namespace MusicBrowser.Entities.Interfaces
{
    public enum EntityKind
    {
        Album,
        Artist,
        Folder,
        Home,
        Playlist,
        Song,
        Unknown
    }

    public abstract class IEntity : BaseModel
    {
        private readonly Dictionary<string, string> _properties;
        private string _sortName;
        private string _description;
        private string _view;
        private string _cacheKey;

        protected IEntity()
        {
            _properties = new Dictionary<string, string>();
            _view = Config.handleEntityView(this.Kind).ToLower();
            this.DefaultBackgroundPath = string.Empty;
            this.Dirty = false;
        }

        public virtual string Path { get; set; }
        public virtual string Title { get; set; }
        public virtual string Summary { get; set; }
        public virtual string IconPath { get; set; }
        public virtual string DefaultIconPath { get; set; }
        public virtual string DefaultBackgroundPath { get; set; }
        public virtual string BackgroundPath { get; set; }
        public virtual string MusicBrainzID { get; set; }
        public virtual IEntity Parent { get; set; }

        // read only (therefore have default values
        public Dictionary<string, string> Properties { get { return _properties; } }
        public virtual EntityKind Kind { get { return EntityKind.Unknown; } }
        public string KindName { get { return Kind.ToString(); } }

        // Calculated/transient
        public int Index { get; set; }
        public virtual string ShortSummaryLine1 { get; set; }
        public long Duration { get; set; }
        public long Children { get; set; }
        public bool Dirty { get; set; }
        public long Version { get; set; }

        // Read Only values
        public string SortName { get { return _sortName; } }
        public new string Description { get { return _description; } }
        public virtual string View { get { return _view; } }

        // Fully implemented
        public Image Background
        {
            get
            {
                // backgrounds are disabled when fan art is disabled
                if (!Config.getInstance().getBooleanSetting("EnableFanArt"))
                {
                    return getImage(String.Empty);
                }
                if (!String.IsNullOrEmpty(BackgroundPath))
                {
                    return getImage(BackgroundPath);
                }
                if (!String.IsNullOrEmpty(DefaultBackgroundPath))
                {
                    return getImage(DefaultBackgroundPath);
                }
                if (Parent != null)
                {
                    if (!Parent.Kind.Equals(EntityKind.Home) && !Kind.Equals(EntityKind.Home))
                    {
                        return Parent.Background;
                    }
                }
                return getImage(String.Empty);
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
                    return getImage(DefaultIconPath); 
                }
                return getImage(IconPath);
            }
        }
        public string CacheKey
        {
            get
            {
                if (String.IsNullOrEmpty(_cacheKey))
                {
                    _cacheKey = Util.Helper.GetCacheKey(Path);
                }
                return _cacheKey;
            }
        }

        public virtual void CalculateValues()
        {
            _description = Config.handleEntityDescription(this);
            _sortName = Config.handleIgnoreWords(_description).ToLower();

            FirePropertiesChanged("Title", "Summary", "Properties", "Index", "ShortSummaryLine1", "ShortSummaryLine2", "Duration", "Children", "SortName", "Description", "Background", "Icon");
        }

        private static Image getImage(string path)
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
            get
            {
                if (_properties.ContainsKey("lfm.playcount") && Config.getInstance().getBooleanSetting("UseInternetProviders"))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(string.Format("Plays: {0:N0}  ", Int32.Parse(_properties["lfm.playcount"])));
                    if (_properties.ContainsKey("lfm.listeners")) 
                        { sb.Append(string.Format("Listeners: {0:N0}  ", Int32.Parse(Properties["lfm.listeners"]))); }
                    if (_properties.ContainsKey("lfm.totalplays")) 
                        { sb.Append(string.Format("Total Plays: {0:N0}  ", Int32.Parse(Properties["lfm.totalplays"]))); }
                    if (_properties.ContainsKey("lfm.loved")) 
                        { if (Properties["lfm.loved"].ToLower() == "true") { sb.Append("LOVED"); } }

                    return "Last.fm  (" + sb.ToString().Trim() + ")";
                }
                return string.Empty;
            }
        }

        public void SetProperty(string key, string value, bool overwrite)
        {
            if (_properties.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(value)) { _properties.Remove(key); }
                if (overwrite) { _properties[key] = value; }
            }
            else
            {
                if (!string.IsNullOrEmpty(value)) { _properties.Add(key, value); }
            }
            Dirty = true;
        }

    }
}
