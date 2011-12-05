using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowSearch : baseActionCommand
    {
        private const string LABEL = "Search";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconSearch";

        public ActionShowSearch(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowSearch()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowSearch(entity);
        }

        public string SearchString { get; set; }

        public override void DoAction(baseEntity entity)
        {
            //Application.GetReference().NavigateToSearch(SearchString, entity);
        }
    }
}
