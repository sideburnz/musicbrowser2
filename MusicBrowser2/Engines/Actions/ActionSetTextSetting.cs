using MusicBrowser.Entities;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Engines.Actions
{
    public class ActionSetTextSetting : baseActionCommand
    {
        private const string LABEL = "Set Text Setting";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconConfig";

        private string _key;
        private string _value;
        private readonly EditableText _editableText = new EditableText();

        public ActionSetTextSetting(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;

            _editableText.PropertyChanged += TextChanged;
        }

        public ActionSetTextSetting()
        {
            Label = LABEL;
            IconPath = ICON_PATH;

            _editableText.PropertyChanged += TextChanged;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionSetTextSetting(entity);
        }

        public string Key 
        { 
            get { return _key; }
            set
            {
                _key = value;
                Value = Util.Config.GetInstance().GetStringSetting(_key);
                _editableText.Value = Value;
                FirePropertyChanged("Value");
            }
        }

        public string Value 
        { 
            get { return _value; } 
            set
            {
                if (_value != value)
                {
                    _value = value;
                    _editableText.Value = value;
                    Util.Config.GetInstance().SetSetting(Key, value);
                    FirePropertyChanged("Value");
                }
            } 
        }

        private void TextChanged(IPropertyObject sender, string property)
        {
            Value = _editableText.Value;
        }

        public EditableText TypingHandler
        {
            get { return _editableText; }
        }

        public void InsertChar(string character)
        {
            _editableText.Value += character;
        }

        public override void DoAction(baseEntity entity)
        {
            Util.Config.GetInstance().SetSetting(Key, Value);
        }
    }
}
