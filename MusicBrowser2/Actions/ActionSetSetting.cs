using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionSetSetting : baseActionCommand
    {
        private const string LABEL = "Set Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private string _key;
        private string _value;

        public ActionSetSetting(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionSetSetting()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSetSetting(entity);
        }

        public string Key 
        { 
            get { return _key; }
            set
            {
                _key = value;
                Value = Util.Config.GetInstance().GetSetting(_key);
                FirePropertyChanged("Value");
            }
        }

        public string Value 
        { 
            get { return _value; } 
            set
            {
                _value = value;
                FirePropertyChanged("Value");
            } 
        }

        public override void DoAction(baseEntity entity)
        {
            Util.Config.GetInstance().SetSetting(Key, Value);
        }
    }
}
