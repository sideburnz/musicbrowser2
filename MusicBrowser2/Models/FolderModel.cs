using System;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Util;
using MusicBrowser.Actions;

namespace MusicBrowser.Models
{
    public class FolderModel : ModelItem
    {
        readonly baseEntity _parentEntity;
        private readonly EntityCollection _entities;
        Int32 _selectedIndex;

        public FolderModel(baseEntity parentEntity, EntityCollection entities)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("FolderModel(kind: " + parentEntity.Kind.ToString() + ", size: " + entities.Count + ")", "start");  
#endif
            _parentEntity = parentEntity;
            _entities = entities;
            CommonTaskQueue.OnStateChanged += BusyStateChanged;
            Busy = CommonTaskQueue.Busy;
        }

        /// <summary>
        /// This is used to display the information in the page header
        /// </summary>
        public baseEntity ParentEntity
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

        public baseEntity SelectedItem
        {
            get
            {
                if (SelectedIndex < 0) { SelectedIndex = 0; }
                if (SelectedIndex > _entities.Count) { SelectedIndex = _entities.Count; }

                if (_entities.Count == 0)
                {
                    baseActionCommand goBack = new ActionPreviousPage(null);
                    goBack.Invoke();
                }
                return _entities[SelectedIndex];
            }
        }

        public static bool ShowClock
        {
            get { return Config.GetInstance().GetBooleanSetting("ShowClock"); }
        }

        public bool Busy { get; set; }

        private void BusyStateChanged(bool busy)
        {
            Busy = busy;
            FirePropertyChanged("Busy");
        }
    }
}
