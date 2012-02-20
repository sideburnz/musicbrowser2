using MusicBrowser.Entities;
using System.Collections;
using System.Collections.Generic;
using MusicBrowser.Engines.Themes;
using MusicBrowser.Util;

namespace MusicBrowser.Actions
{
    // this is a heavily modified action really intended only as the model for spinners

    public class ActionSetOptionList : baseActionCommand
    {
        private const string LABEL = "Set Theme";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private int _index;
        private readonly List<string> _options = new List<string>();
        private readonly Config _config = Config.GetInstance();

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

        public List<string> Options
        {
            set
            {
                _options.Clear();
                _options.AddRange(value);
                if (!string.IsNullOrEmpty(Key)) { _index = _options.IndexOf(_config.GetStringSetting(Key)); }
                if (_index < 0) { _index = 0; }
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
                Util.Config.GetInstance().SetSetting(_key, SelectedItem);
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
                Util.Config.GetInstance().SetSetting(_key, SelectedItem);
            }
            Invoke();
        }

        public string SelectedItem
        {
            get 
            {
                return _options[_index];
            }
            set
            {
                _index = _options.IndexOf(value);
                if (_index < 0) { _index = 0; }
            }
        }

        public override void DoAction(baseEntity entity)
        {
            FirePropertyChanged("SelectedItem");
        }
    }
}
