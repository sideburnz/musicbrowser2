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

        public string Label { get; set; }

        public void Execute(Entity entity, String caller)
        {
            string title;
            if (entity == null)
            {
                title = "none defined";
            }
            else
            {
                title = entity.Title;
            }

            Logging.Logger.Verbose(String.Format("Action: {0}, Entity: {1}", Description, title), "start");

            Statistics.Hit("Action." + Description + "." + caller);

            try
            {
                DoAction(entity);
            }
            catch(Exception e)
            {
                Logging.Logger.Error(e);
            }

            Logging.Logger.Verbose(String.Format("Action: {0}, Entity: {1}", Description, title), "finish");
        }

        public virtual void DoAction(Entity entity) {}

        public string IconPath { get; set; }

        public Image Icon
        {
            get { return new Image(IconPath); }
        }
    }
}
