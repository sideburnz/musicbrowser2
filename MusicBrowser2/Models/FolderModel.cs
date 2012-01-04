using System;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Util;
using MusicBrowser.Actions;
using MusicBrowser.Models.Keyboard;
using MusicBrowser.Providers;

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

            _parentEntity.OnPropertyChanged += new baseEntity.ChangedPropertyHandler(_parentEntity_OnPropertyChanged);

            int i = 0;

            int ratio1to1 = 0;
            int ratio11to2 = 0;
            int ratio16to9 = 0;
            int ratio2to3 = 0;

            foreach (baseEntity e in entities)
            {
                try
                {
                    ImageRatio r = ImageProvider.Ratio(new System.Drawing.Bitmap(e.ThumbPath));
                    if (r != ImageRatio.RatioUncommon)
                    {
                        i++;

                        switch (r)
                        {
                            case ImageRatio.Ratio11to2:
                                ratio11to2++; break;
                            case ImageRatio.Ratio16to9:
                                ratio16to9++; break;
                            case ImageRatio.Ratio2to3:
                                ratio2to3++; break;
                            case ImageRatio.Ratio1to1:
                                ratio1to1++; break;
                        }

                    }
                    if (i > 10)
                    {
                        break;
                    }
                }
                catch { }
            }

            if (ratio1to1 > ratio2to3 && ratio1to1 > ratio16to9 && ratio1to1 > ratio11to2)
            {
                ReferenceRatio = 1;
            }
            else if (ratio2to3 > ratio1to1 && ratio2to3 > ratio16to9 && ratio2to3 > ratio11to2)
            {
                ReferenceRatio = 2 / 3.00;
            }
            else if (ratio16to9 > ratio1to1 && ratio16to9 > ratio2to3 && ratio16to9 > ratio11to2)
            {
                ReferenceRatio = 16 / 9.00;
            }
            else if (ratio11to2 > ratio1to1 && ratio11to2 > ratio2to3 && ratio11to2 > ratio16to9)
            {
                ReferenceRatio = 11 / 2.00;
            }
            else
            {
                ReferenceRatio = 1;
            }
        }

        void _parentEntity_OnPropertyChanged(string property)
        {
            switch (property.ToLower())
            {
                case "thumbsize":
                    {
                        FirePropertyChanged("ReferenceSize");
                        break;
                    }
            }
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

        private double _refRatio = 1;
        private double ReferenceRatio
        {
            get
            {
                return _refRatio;
            }
            set
            {
                if (value != _refRatio)
                {
                    _refRatio = value;
                    FirePropertyChanged("ReferenceSize");
                }
            }
        }

        [MarkupVisible]
        public Size ReferenceSize
        {
            get
            {
                int i = _parentEntity.ThumbSize;
                return new Size((int)(i * ReferenceRatio), i);
            }
        }

        public Size FixedReferenceSize
        {
            get
            {
                int i = 127;
                return new Size((int)(i * ReferenceRatio), i);
            }
        }
    }
}
