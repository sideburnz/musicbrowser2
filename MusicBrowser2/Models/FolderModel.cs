using System;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Models.Keyboard;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using MusicBrowser.Util;

namespace MusicBrowser.Models
{
    public class FolderModel : ModelItem
    {
        private readonly baseEntity _parentEntity;
        private Int32 _selectedIndex;
        private readonly IKeyboardHandler _keyboard;
        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public FolderModel(baseEntity parentEntity, EntityCollection entities, IKeyboardHandler keyboard)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("FolderModel(kind: " + parentEntity.Kind + ", size: " + entities.Count + ")", "start");  
#endif
            _keyboard = keyboard;
            _keyboard.RawDataSet = entities;
            _parentEntity = parentEntity;
            IKeyboardHandler.OnDataChanged += KeyboardHandler;
            CommonTaskQueue.OnStateChanged += BusyStateChanged;
            Busy = CommonTaskQueue.Busy;

            _parentEntity.OnPropertyChanged += _parentEntity_OnPropertyChanged;
            Config.OnSettingUpdate += Config_OnSettingUpdate;

            int i = 0;

            int ratio1To1 = 0;
            int ratio11To2 = 0;
            int ratio16To9 = 0;
            int ratio2To3 = 0;

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
                            case ImageRatio.Ratio11To2:
                                ratio11To2++; break;
                            case ImageRatio.Ratio16To9:
                                ratio16To9++; break;
                            case ImageRatio.Ratio2To3:
                                ratio2To3++; break;
                            case ImageRatio.Ratio1To1:
                                ratio1To1++; break;
                        }

                    }
                }
                catch 
                {
                    if (e.InheritsFrom<Video>())
                    {
                        i++;
                        ratio2To3++;
                    }
                }
                if (i > 10)
                {
                    break;
                }
            }

            if (ratio1To1 > ratio2To3 && ratio1To1 > ratio16To9 && ratio1To1 > ratio11To2)
            {
                ReferenceRatio = 1;
            }
            else if (ratio2To3 > ratio1To1 && ratio2To3 > ratio16To9 && ratio2To3 > ratio11To2)
            {
                ReferenceRatio = 2 / 3.00;
            }
            else if (ratio16To9 > ratio1To1 && ratio16To9 > ratio2To3 && ratio16To9 > ratio11To2)
            {
                ReferenceRatio = 16 / 9.00;
            }
            else if (ratio11To2 > ratio1To1 && ratio11To2 > ratio2To3 && ratio11To2 > ratio16To9)
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
                case "backgroundpaths":
                    {
                        FirePropertyChanged("Background");
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
        /// 

        public void RedrawItems()
        {
            FirePropertyChanged("EntityList");
            FirePropertyChanged("ReferenceSize");
            FirePropertyChanged("ReferenceHeight");
        }

        public EntityVirtualList EntityList
        {
            get
            {
                return new EntityVirtualList(_keyboard.DataSet, _parentEntity.ViewState.SortField, _parentEntity.ViewState.SortAscending);
            }
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
                if (SelectedIndex < 0) { SelectedIndex = 1; }
                if (SelectedIndex > _keyboard.DataSet.Count) { SelectedIndex = _keyboard.DataSet.Count; }

                if (_keyboard.DataSet.Count == 0)
                {
                    baseActionCommand goBack = new ActionPreviousPage(null);
                    goBack.Invoke();
                }
                return _keyboard.DataSet[SelectedIndex];
            }
        }

        void Config_OnSettingUpdate(string Key)
        {
            switch (Key.ToLower())
            {
                case "showclock":
                    FirePropertyChanged("ShowClock");
                    break;
                case "views.ishorizontal":
                    FirePropertyChanged("IsHorizontal");
                    break;
                case "showthumbs":
                    FirePropertyChanged("ShowThumbs");
                    break;

            }
        }

        public static bool ShowClock
        {
            get { return Config.GetInstance().GetBooleanSetting("ShowClock"); }
        }

        public bool ShowThumbs
        {
            get { return Config.GetInstance().GetBooleanSetting("ShowThumbs"); ; }
        }

        public bool IsHorizontal
        {
            get { return Config.GetInstance().GetBooleanSetting("Views.IsHorizontal"); }
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
                    FirePropertyChanged("ReferenceHeight");
                }
            }
        }

        [MarkupVisible]
        public Size ReferenceSize
        {
            get
            {
                int i = _parentEntity.ViewState.ThumbSize;
                return new Size((int)(i * ReferenceRatio), i);
            }
        }

        [MarkupVisible]
        public Size ReferenceHeight
        {
            get
            {
                return new Size(0, _parentEntity.ViewState.ThumbSize);
            }
        }

        [MarkupVisible]
        public Image Background
        {
            get
            {
                if (_parentEntity.BackgroundPaths == null || _parentEntity.BackgroundPaths.Count == 0)
                {
                    return Helper.GetImage(String.Empty);
                }
                if (_parentEntity.BackgroundPaths.Count == 1)
                {
                    return Helper.GetImage(_parentEntity.BackgroundPaths[0]);
                }
                int i = Rnd.Next(_parentEntity.BackgroundPaths.Count);
                return Helper.GetImage(_parentEntity.BackgroundPaths[i]);
            }
        }
    }
}
