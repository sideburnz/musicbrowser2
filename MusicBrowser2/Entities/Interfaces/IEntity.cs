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
        Unknown,
        NotDetermined
    }

    public abstract class IEntity : BaseModel
    {
        private readonly Dictionary<string, string> _properties;
        private string _sortName;
        private string _description;
        private string _view;
        private string _cacheKey;
        private long _version;

        public IEntity()
        {
            _properties = new Dictionary<string, string>();
            _view = Config.handleEntityView(Kind).ToLower();
            DefaultBackgroundPath = string.Empty;
            Dirty = false;
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
        public virtual EntityKind Kind { get { return EntityKind.NotDetermined; } }
        public string KindName { get { return Kind.ToString(); } }

        // Calculated/transient
        public int Index { get; set; }
        public virtual string ShortSummaryLine1 { get; set; }
        public string ShortSummaryLine2 { get; set; }
        public long Duration { get; set; }
        public long Children { get; set; }
        public bool Dirty { get; set; }

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
                if (!Parent.Kind.Equals(EntityKind.Home) && !Parent.Kind.Equals(EntityKind.Unknown))
                {
                    return Parent.Background;
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
        public long Version
        {
            set
            {
                _version = value;
            }
            get
            {
                if (_version == 0)
                { 
                    _version = Util.Helper.ParseVersion("0.0.0.0"); 
                }
                return _version;
            }
        }

        public virtual void CalculateValues()
        {
            _description = Config.handleEntityDescription(this);
            _sortName = Config.handleStartingThe(_description).ToLower();
            FirePropertiesChanged("Title", "Summary", "Properties", "Index", "ShortSummaryLine1", "ShortSummaryLine2", "Duration", "Children", "SortName", "Description", "Background", "Icon");
        }

        private Image getImage(string path)
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
    }
}
