using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    // this is a heavily modified action really intended only as the model for spinners

    public class ActionSetNumericSetting : baseActionCommand
    {
        private const string LABEL = "Set Numeric Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";
        private const int DEFAULT_STEPPING = 5;
        private const int LOWER_LIMIT = 0;
        private const int UPPER_LIMIT = 100;

        private string _key;
        private int _value;

        public ActionSetNumericSetting(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Stepping = DEFAULT_STEPPING;
            Upper = UPPER_LIMIT;
            Lower = LOWER_LIMIT;
        }

        public ActionSetNumericSetting()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Stepping = DEFAULT_STEPPING;
            Upper = UPPER_LIMIT;
            Lower = LOWER_LIMIT;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSetNumericSetting(entity);
        }

        public string Key 
        { 
            get { return _key; }
            set
            {
                _key = value;
                Value = Util.Config.GetIntSetting(_key);
                FirePropertyChanged("Value");
            }
        }

        public int Value 
        { 
            get { return _value; } 
            set
            {
                if (value <= Upper && value >= Lower)
                {
                    _value = value;
                    Util.Config.SetSetting(_key, _value.ToString());
                    FirePropertyChanged("Value");
                }
            } 
        }

        public int Stepping { get; set; }
        public int Lower { get; set; }
        public int Upper { get; set; }

        public void Increment()
        {
            Value = Value + Stepping;
        }

        public void Decrement()
        {
            Value = Value - Stepping;
        }

        public override void DoAction(baseEntity entity)
        {
            Util.Config.SetSetting(Key, Value.ToString());
        }
    }
}
