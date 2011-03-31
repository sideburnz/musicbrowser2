using System;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;
using MusicBrowser.Entities.Interfaces;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using System.Text.RegularExpressions;
using System.Linq;
using MusicBrowser.MediaCentre;

namespace MusicBrowser.Models
{
    public class FolderModel : BaseModel
    {

        readonly IEntity _parentEntity;
        private readonly EntityCollection _entities;
        private readonly EntityCollection _fullentities;
        Int32 _selectedIndex = 0;
        readonly Breadcrumbs _crumbs;
        IEntity _PopupPlayContext = new Unknown();
        private readonly EditableText _remoteFilter = new EditableText();
        private int _matches;

        public FolderModel(IEntity parentEntity, Breadcrumbs crumbs, EntityCollection entities)
        {
#if DEBUG
            Logging.Logger.Verbose("FolderModel(kind: " + parentEntity.Kind.ToString() + ", path: " + crumbs.Path + ", size: " + entities.Count + ")", "start");  
#endif
            _crumbs = new Breadcrumbs(crumbs);
            _crumbs.Add(parentEntity);
            _parentEntity = parentEntity;
            _entities = entities;
            _fullentities = new EntityCollection();
            _fullentities.AddRange(_entities);
            _matches = _fullentities.Count;

            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
            MediaContext.GetInstance().OnPlayStateChanged += PlayStateChanged;       
        }

        /// <summary>
        /// This is used to display the information in the page header
        /// </summary>
        public IEntity ParentEntity
        {
            get 
            {
                return _parentEntity; 
            }
        }

        /// <summary>
        /// This is the list of items to display on the page
        /// </summary>
        public EntityCollection EntityCollection
        {
            get { return _entities; }
        }

        /// <summary>
        /// This indicates the item that is currently selected
        /// </summary>
        public Int32 SelectedIndex
        {
            get { return _selectedIndex; }
            set { 
                _selectedIndex = value;
                FirePropertiesChanged("SelectedIndex", "SelectedIndex");
            }
        }

        public IEntity SelectedItem
        {
            get
            {
                if (_entities.Count > 0)
                {
                    return _entities[SelectedIndex];
                }
                return new Unknown();
            }
        }

        public Breadcrumbs Crumbs()
        {
            return _crumbs;
        }

        public bool ShowPopupPlay
        {
            get { return (_PopupPlayContext.Kind != EntityKind.Unknown); }
        }

        public IEntity GetPopupPlayContext
        {
            get { return _PopupPlayContext; }
        }

        public void SetPopupPlayContext(IEntity entity)
        {
            _PopupPlayContext = entity;
            FirePropertiesChanged("ShowPopupPlay");
        }

        public void ClearPopupPlayContext()
        {
            SetPopupPlayContext(new Unknown());
        }

        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        {
            //TODO: implement parallel extentions when migrating to .Net4

            if (property == "Value")
            {
                // if it's resetting the filter, then just shortcut and load the
                // entity list with the full set of data
                if (string.IsNullOrEmpty(_remoteFilter.Value) || (_remoteFilter.Value.Contains('\\')))
                {
                    _entities.Clear();
                    _entities.AddRange(_fullentities);
                    FirePropertiesChanged("Matches", "EntityCollection", "FullSize");
                }

                _matches = 0;
                int listSize = _fullentities.Count;
                Regex regex = new Regex("\\b" + _remoteFilter.Value.ToLower());
                EntityCollection temp = new EntityCollection();

                //this is slower than the foreach loop, in .Net4 make it parallel
                
                foreach (IEntity item in _fullentities)
                {
                    if (regex.IsMatch(item.SortName))
                    {
                        temp.Add(item);
                    }
                }

                _matches = temp.Count;
                if (_matches > 0)
                {
                    temp.IndexItems();
                    _entities.Clear();
                    _entities.AddRange(temp);
                    FirePropertiesChanged("Matches", "EntityCollection", "FullSize");
                }
                else
                {
                    _remoteFilter.Value = String.Empty;
                }
            }
        }

        [MarkupVisible]
        public EditableText RemoteTyper
        {
            get { return _remoteFilter; }
        }

        public string Matches
        {
            get { return _matches.ToString(); }
        }

        public string FullSize
        {
            get
            {
                if (_matches != 0 & _matches != _fullentities.Count)
                {
                    return "(" + _fullentities.Count + ")";
                }
                return string.Empty;
            }
        }

        public static bool ShowClock
        {
            get { return Config.getInstance().getBooleanSetting("ShowClock"); }
        }

        public static bool isPlaying
        {
            get { return MediaContext.GetInstance().PlayState == PlayState.Playing; }
        }

        public static bool isPaused
        {
            get { return MediaContext.GetInstance().PlayState == PlayState.Paused; }
        }

        void PlayStateChanged(object obj)
        {
            FirePropertiesChanged("isPlaying", "isPaused");
        }
    }
}
