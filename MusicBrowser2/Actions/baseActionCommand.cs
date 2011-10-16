using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Models;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Actions
{
    public abstract class baseActionCommand : BaseModel
    {
        private readonly Entity _entity;

        public baseActionCommand(Entity entity)
        {
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/nullImage";
            Label = this.GetType().Name;
            _entity = entity;
        }

        public string Label { get; set; }

        public void Execute()
        {
            string title;
            if (_entity == null)
            {
                title = "none defined";
            }
            else
            {
                title = _entity.Title;
            }

            Logging.Logger.Debug(String.Format("Action: {0}, Entity: {1}", Description, title));

            Statistics.Hit("Action." + Description);

            try
            {
                DoAction(_entity);
                ActionsModel.GetInstance.Visible = false;
            }
            catch(Exception e)
            {
                Logging.Logger.Error(e);
            }
        }

        public abstract void DoAction(Entity entity);

        public string IconPath { get; set; }

        public Image Icon
        {
            get { return new Image(IconPath); }
        }
    }
}
