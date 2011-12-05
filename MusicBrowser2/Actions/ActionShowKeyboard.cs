using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter.Hosting;

namespace MusicBrowser.Actions
{
    public class ActionShowKeyboard : baseActionCommand
    {
        private const string LABEL = "Settings";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/Keyboard";

        public ActionShowKeyboard(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowKeyboard()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowKeyboard(entity);
        }

        public EditableText editableText { get; set; }

        public int MaxLength { get; set; }

        public override void DoAction(baseEntity entity)
        {
            AddInHost.Current.MediaCenterEnvironment.ShowOnscreenKeyboard(editableText, false, MaxLength);
        }
    }
}
