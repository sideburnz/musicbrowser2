using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Providers.Transport;
using MusicBrowser.Util;

namespace MusicBrowser.Models
{
    public class FolderModel : ModelItem
    {

        readonly Entity _parentEntity;
        private readonly EntityCollection _entities;
        private readonly EntityCollection _fullentities;
        Int32 _selectedIndex;
        readonly Breadcrumbs _crumbs;
        Entity _popupPlayContext = null;
        private readonly EditableText _remoteFilter = new EditableText();
        private int _matches;

        private readonly bool _isHome;

        public FolderModel(Entity parentEntity, Breadcrumbs crumbs, EntityCollection entities)
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

            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;    
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
                FirePropertyChanged("SelectedIndex");
            }
        }

        //TODO: ensure this is clearing down all the caches, none of the images appear to update when this is called
        public void ForceRefresh()
        {
            if (_parentEntity.Kind == EntityKind.Home) { return; }

            int itemCount = 1;
            ICacheEngine cacheEngine = CacheEngine.CacheEngineFactory.GetCacheEngine();

            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_parentEntity.Path);
            IEnumerable<IDataProvider> providers = MetadataProviderList.GetProviders();

            CommonTaskQueue.Enqueue(new MetadataProviderList(_parentEntity, true), true);

            foreach (FileSystemItem item in items)
            {
                // don't waste time on the item
                if (!Util.Helper.IsEntity(item.FullPath)) { continue; }
                if (item.Name.ToLower() == "metadata") { continue; }

                // remove the cache (force it to rediscover the type)
                string key = Util.Helper.GetCacheKey(item.FullPath);
                cacheEngine.Delete(key);
                CacheEngine.NearLineCache.GetInstance().Remove(key);

                // process the item in context
                Entity entity = EntityFactory.GetItem(item);
                if (entity == null || entity.Kind.Equals(EntityKind.Folder)) { continue; }

                // fire off the metadata providers
                
                CommonTaskQueue.Enqueue(new MetadataProviderList(entity, true), true);
                itemCount++;
            }

            UINotifier.GetInstance().Message = "refreshing metadata for " + itemCount + " items";
        }

        public Entity SelectedItem
        {
            get
            {
                if (_entities.Count > 0)
                {
                    return _entities[SelectedIndex];
                }
                return null;
            }
        }

        public Breadcrumbs Crumbs()
        {
            return _crumbs;
        }

        public bool ShowPopupPlay
        {
            get { return !(_popupPlayContext == null); }
        }

        public Entity GetPopupPlayContext
        {
            get { return _popupPlayContext; }
        }

        public void SetPopupPlayContext(Entity entity)
        {
            _popupPlayContext = entity;
            FirePropertyChanged("ShowPopupPlay");
        }

        public void ClearPopupPlayContext()
        {
            SetPopupPlayContext(null);
        }

        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        {
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
                Regex regex = new Regex("\\b" + _remoteFilter.Value.ToLower());
                EntityCollection temp = new EntityCollection();
                temp.AddRange(_fullentities.Where(item => regex.IsMatch(item.SortName)));

                _matches = temp.Count;
                if (_matches > 0)
                {
                    temp.IndexItems();
                    _entities.Clear();
                    _entities.AddRange(temp);
                    FirePropertyChanged("Matches");
                    FirePropertyChanged("EntityCollection");
                    FirePropertyChanged("FullSize");
                }
                else
                {
                    _remoteFilter.Value = String.Empty;
                }
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

        private void RefreshEntities()
        {
            _entities.Clear();
            _entities.AddRange(_fullentities);
            _matches = _fullentities.Count;
            _entities.IndexItems();
            _remoteFilter.Value = String.Empty;
            FirePropertyChanged("Matches");
            FirePropertyChanged("EntityCollection");
            FirePropertyChanged("FullSize");
        }

        public void TransportCommand(string command)
        {
            Logging.Logger.Debug("TransportCommand: " + command);

            switch (command.ToLower())
            {
                case "next": Transport.GetTransport().Next(); break;
                case "prev": Transport.GetTransport().Previous(); break;
                case "stop": Transport.GetTransport().Stop(); break;
                case "playpause": Transport.GetTransport().PlayPause(); break;
            }
        }

    }
}
