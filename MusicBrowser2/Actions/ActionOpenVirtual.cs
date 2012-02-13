using MusicBrowser.Entities;
using System.IO;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MusicBrowser.Actions
{
    public class ActionOpenVirtual : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconOpen";

        private string _title;

        public ActionOpenVirtual(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionOpenVirtual()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Label = LABEL + " " + value;
            }
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOpenVirtual(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            baseEntity e = entity;
            if (e == null)
            {
                e = new Virtual();
                e.Title = Title;
            }
            MusicBrowser.Application.GetReference().Navigate(e);
        }

    }
}
