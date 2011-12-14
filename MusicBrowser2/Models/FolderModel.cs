using System;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Util;
using MusicBrowser.Actions;
using MusicBrowser.Models.Keyboard;

namespace MusicBrowser.Models
{
    public class FolderModel : ModelItem
    {
        readonly baseEntity _parentEntity;
        Int32 _selectedIndex;
        IKeyboardHandler _keyboard;

        public FolderModel(baseEntity parentEntity, EntityCollection entities, IKeyboardHandler keyboard)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("FolderModel(kind: " + parentEntity.Kind.ToString() + ", size: " + entities.Count + ")", "start");  
#endif
            _keyboard = keyboard;
            _keyboard.RawDataSet = entities;
            _parentEntity = parentEntity;
            IKeyboardHandler.OnDataChanged += KeyboardHandler;
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

        public string KeyedValue
        {
            get { return _keyboard.Value; }
            set { _keyboard.Value = value; }
        }

        public Application application { get; set; }

        public string Matches
        {
            get
            {
                return _keyboard.DataSet.Count.ToString();
            }
        }

        /// <summary>
        /// This is the list of items to display on the page
        /// </summary>
        public EntityVirtualList EntityList
        {
            get { return new EntityVirtualList(_keyboard.DataSet, _parentEntity.SortField); }
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
                if (SelectedIndex > _keyboard.DataSet.Count) { SelectedIndex = _keyboard.DataSet.Count; }

                if (_keyboard.DataSet.Count == 0)
                {
                    baseActionCommand goBack = new ActionPreviousPage(null);
                    goBack.Invoke();
                }
                return _keyboard.DataSet[SelectedIndex];
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

        public int JILIndex { get; set; }

        private void KeyboardHandler(string key)
        {
            if (key.Equals("DataSet", StringComparison.InvariantCultureIgnoreCase))
            {
                FirePropertyChanged("EntityList");
            }
            if (key.Equals("Value", StringComparison.InvariantCultureIgnoreCase))
            {
                FirePropertyChanged("KeyedValue");
            }
            if (key.Equals("Index", StringComparison.CurrentCultureIgnoreCase))
            {
                JILIndex = _keyboard.Index;
                FirePropertyChanged("JILIndex");
            }
        }
    }
}
