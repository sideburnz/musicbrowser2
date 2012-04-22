using System;
using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Actions
{
    // this is a heavily modified action really intended only as the model for spinners

    public class ActionSetOptionList : baseActionCommand
    {
        private const string LABEL = "Set ThemeLoader";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private int _index;
        private readonly List<string> _options = new List<string>();
        private readonly Config _config = Config.GetInstance();
        private string _selectedValue;

        private string _key;

        public ActionSetOptionList(baseEntity entity) { }

        public ActionSetOptionList()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                _index = 0;
            }
        }

        private void SetOption(string item)
        {
            for (int index = 0; index < _options.Count; index++) 
            {
                if (item.Equals(item, StringComparison.OrdinalIgnoreCase))
                {
                    _index = index;
                    _selectedValue = item;
                    FirePropertyChanged("SelectedItem");
                    return;
                }
            }
            _index = 0;
            _selectedValue = String.Empty;
            FirePropertyChanged("SelectedItem");
        }

        public List<string> Options
        {
            set
            {
                _options.Clear();
                _options.AddRange(value);

                if (!String.IsNullOrEmpty(_selectedValue))
                {
                    SetOption(_selectedValue);
                }
                else if (!string.IsNullOrEmpty(Key))
                {
                    SetOption(_config.GetStringSetting(Key));
                }
                FirePropertyChanged("SelectedItem");
            }
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSetOptionList(entity);
        }

        public void Increment()
        {
            _index++;
            if (_index >= _options.Count)
            {
                _index = 0;
            }
            if (!string.IsNullOrEmpty(Key))
            {
                Config.GetInstance().SetSetting(_key, SelectedItem);
            }
            Invoke();
        }

        public void Decrement()
        {
            _index--;
            if (_index < 0 )
            {
                _index = _options.Count - 1;
            }
            if (!string.IsNullOrEmpty(Key))
            {
                Config.GetInstance().SetSetting(_key, SelectedItem);
            }
            Invoke();
        }

        public string SelectedItem
        {
            get 
            {
                string ret;
                try
                {
                    ret = _options[_index];
                }
                catch
                {
                    ret = _options[0];
                }
                return ret;
            }
            set
            {
                SetOption(value);
                _selectedValue = value;
            }
        }

        public override void DoAction(baseEntity entity)
        {
            FirePropertyChanged("SelectedItem");
        }
    }
}
