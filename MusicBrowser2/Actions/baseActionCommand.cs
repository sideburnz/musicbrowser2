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
    public abstract class baseActionCommand : BaseModel, ICommand
    {
         public baseActionCommand()
        {
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/nullImage";
            Label = this.GetType().Name;
            Available = true;
        }

        public Entity Entity { get; set; }

        public string Label { get; set; }

        public bool KeepMenuShowingAfterExecution { get; set; }

        public void Invoke()
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
                ActionsModel.GetInstance.Visible = KeepMenuShowingAfterExecution;
            }
            catch(Exception e)
            {
                Logging.Logger.Error(e);
            }

            if (Invoked != null)
            {
                Invoked(this, new EventArgs());
            }
        }

        public event EventHandler Invoked;

        public abstract void DoAction(Entity entity);

        public string IconPath { get; set; }

        public Image Icon
        {
            get { return new Image(IconPath); }
        }

        public bool Available { get; set; }

    }
}
