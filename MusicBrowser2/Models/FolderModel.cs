using System;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using MusicBrowser.MediaCentre;
using MusicBrowser.Providers.CD;
using MusicBrowser.Providers.Transport;

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

        private readonly bool _isHome;
        private IList<CDDrive> _CDDrives = new List<CDDrive>();

        public FolderModel(IEntity parentEntity, Breadcrumbs crumbs, EntityCollection entities)
        {
#if DEBUG
            Logging.Logger.Verbose("FolderModel(kind: " + parentEntity.Kind.ToString() + ", path: " + crumbs.Path + ", size: " + entities.Count + ")", "start");  
#endif
            _crumbs = new Breadcrumbs(crumbs);
            _crumbs.Add(parentEntity);
            _parentEntity = parentEntity;
            _entities = entities;

            _isHome = parentEntity.Kind.Equals(EntityKind.Home);
            
            _fullentities = new EntityCollection();
            _fullentities.AddRange(_entities);
            _matches = _fullentities.Count;

//          if (_isHome) { DealWithCDs(); }

            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;    
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
                Logging.Logger.Debug("filter = " + _remoteFilter.Value);

                // if it's resetting the filter, then just shortcut and load the
                // entity list with the full set of data
                if (string.IsNullOrEmpty(_remoteFilter.Value) || (_remoteFilter.Value.Contains('\\')))
                {
                    RefreshEntities();
                    return;
                }

                _matches = 0;
                int listSize = _fullentities.Count;
                Regex regex = new Regex("\\b" + _remoteFilter.Value.ToLower());
                EntityCollection temp = new EntityCollection();
                
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
            get { return Transport.getTransport().State == MusicBrowser.Providers.Transport.PlayState.Playing; }
        }

        public static bool isPaused
        {
            get { return Transport.getTransport().State == MusicBrowser.Providers.Transport.PlayState.Paused; }
        }

        private void DealWithCDs()
        {
            /* for each CD, register listeners for the on insert and remove
             * the insert should put the disc into the home list and fetch metadata
             * remove should remove from the list */

            if (_isHome & Config.getInstance().getBooleanSetting("ShowCDs"))
            {
                foreach (char letter in CDDrive.GetCDDriveLetters())
                {
                    CDDrive d = new CDDrive();
                    d.Open(letter);
                    _CDDrives.Add(d);
                    d.CDInserted += OnCDInserted;
                    d.CDRemoved += OnCDRemoved;
                    
                    OnCDInserted(d, null);
                }
            }
        }

        private void OnCDInserted(object sender, EventArgs e)
        {
            Logging.Logger.Debug("CD inserted: " + ((CDDrive)sender).Letter);

            _fullentities.Insert(0, new Entities.Kinds.Disc(((CDDrive)sender).Letter));
            RefreshEntities();
        }

        private void OnCDRemoved(object sender, EventArgs e)
        {
            CDDrive obj = (CDDrive)sender;

            Logging.Logger.Debug("CD removed: " + obj.Letter);

            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entities[i].Kind.Equals(EntityKind.Disc))
                {
                    if (((Disc)_entities[i]).Letter == obj.Letter)
                    {
                        _fullentities.RemoveAt(i);
                        RefreshEntities();
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void RefreshEntities()
        {
            _entities.Clear();
            _entities.AddRange(_fullentities);
            _matches = _fullentities.Count;
            _entities.IndexItems();
            _remoteFilter.Value = String.Empty;
            FirePropertiesChanged("Matches", "EntityCollection", "FullSize");
        }

        public void TransportCommand(string command)
        {
            Logging.Logger.Debug("TransportCommand: " + command);

            switch (command.ToLower())
            {
                case "next": MusicBrowser.Providers.Transport.Transport.getTransport().Next(); break;
                case "prev": MusicBrowser.Providers.Transport.Transport.getTransport().Previous(); break;
                case "stop": MusicBrowser.Providers.Transport.Transport.getTransport().Stop(); break;
                case "playpause": MusicBrowser.Providers.Transport.Transport.getTransport().PlayPause(); break;
            }
        }

    }
}
