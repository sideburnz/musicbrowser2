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
         public baseActionCommand()
        {
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/nullImage";
            Label = this.GetType().Name;
        }

        public Entity Entity { get; set; }

        public string Label { get; set; }

        public void Execute()
        {
            string title;
            if (Entity == null)
            {
                title = "none defined";
            }
            else
            {
                title = Entity.Title;
            }

            Logging.Logger.Debug(String.Format("Action: {0}, Entity: {1}", Label, title));

            Statistics.Hit("Action." + Label);

            try
            {
                DoAction(Entity);
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
