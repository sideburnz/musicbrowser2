﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Util;

namespace MusicBrowser.Models
{
    public class FolderModel : ModelItem
    {

        readonly Entity _parentEntity;
        private readonly EntityCollection _entities;
        Int32 _selectedIndex;
        private readonly bool _isHome;

        public FolderModel(Entity parentEntity, EntityCollection entities)
        {
#if DEBUG
            Logging.Logger.Verbose("FolderModel(kind: " + parentEntity.Kind.ToString() + ", size: " + entities.Count + ")", "start");  
#endif

            _parentEntity = parentEntity;
            _entities = entities;
            _isHome = (parentEntity.Kind == EntityKind.Home);

            Config.OnSettingUpdate += SettingsChanged;
            CommonTaskQueue.OnStateChanged += BusyStateChanged;
            Busy = CommonTaskQueue.Busy;
        }

        /// <summary>
        /// This is used to display the information in the page header
        /// </summary>
        public Entity ParentEntity
        {
            get 
            {
                return _parentEntity; 
            }
        }

        public Application application { get; set; }

        public string Matches
        {
            get
            {
                return _entities.Count.ToString();
            }
        }

        /// <summary>
        /// This is the list of items to display on the page
        /// </summary>
        public EntityVirtualList EntityList
        {
            get { return new EntityVirtualList(_entities); }
        }

        /// <summary>
        /// This indicates the item that is currently selected
        /// </summary>
        public Int32 SelectedIndex
        {
            get { return _selectedIndex; }
            set { 
                _selectedIndex = value;
                FirePropertyChanged("SelectedIndex");
            }
        }

        public Entity SelectedItem
        {
            get
            {
                if (SelectedIndex < 0) { SelectedIndex = 0; }
                if (SelectedIndex > _entities.Count) { SelectedIndex = _entities.Count; }

                if (_entities.Count == 0)
                {
                    return new Entity() { Kind = EntityKind.Home };
                }
                return _entities[SelectedIndex];
            }
        }

        [MarkupVisible]
        public string Version
        {
            get
            {
                if (Config.GetInstance().GetBooleanSetting("ShowVersion"))
                {
                    return "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                }
                return string.Empty;
            }
        }

        public static bool ShowClock
        {
            get { return Config.GetInstance().GetBooleanSetting("ShowClock"); }
        }

        public static bool isPlaying
        {
            get { return Transport.GetTransport().State == PlayState.Playing; }
        }

        public static bool isPaused
        {
            get { return Transport.GetTransport().State == PlayState.Paused; }
        }

        //TODO: these should be Actions?
        //public void NavigateToVirtual(string name)
        //{
        //    Entity virt = new Entity() 
        //    { 
        //        Kind = EntityKind.Virtual, 
        //        Path = name,
        //        Label = name,
        //        Title = name 
        //    };
        //    application.Navigate(virt);
        //}

        //public void NavigateToGroup(string name)
        //{
        //    Entity group = new Entity() 
        //    { 
        //        Kind = EntityKind.Group, 
        //        Title = name,
        //        Label = name,
        //        Path = name
        //    };
        //    application.Navigate(group);
        //}

        public bool Busy { get; set; }

        private void BusyStateChanged(bool busy)
        {
            Busy = busy;
            FirePropertyChanged("Busy");
        }

        private void SettingsChanged(string key)
        {
            if (key.ToLower() == "view") { FirePropertyChanged("View"); }
        }

        public string View 
        { 
            get 
            { 
                return Config.GetInstance().GetStringSetting(ParentEntity.KindName + ".View").ToLower(); 
            } 
        }


    }
}
