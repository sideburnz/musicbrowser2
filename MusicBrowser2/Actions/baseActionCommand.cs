using System;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Actions
{
    /// <summary>
    /// This is the base class that all UI commands should be derived from.
    /// 
    /// It has the following features:
    /// - Implements the MediaCentre ICommand interface which means that the commands can be called from MCML in a consistent manner
    /// - Implements the Command pattern, executing is as simple as calling the Invoke method
    /// - Implements the Template pattern, the built-in steps perform logging for telemetary and also wraps error handling
    /// </summary>
    public abstract class baseActionCommand : BaseModel, ICommand
    {
        public baseActionCommand()
        {
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/nullImage";
            Label = this.GetType().Name;
            Available = true;
        }

        /// <summary>
        /// Sets the entity the Action is to be performed on, when relevant
        /// </summary>
        public baseEntity Entity { get; set; }

        /// <summary>
        /// Sets the name that is used for logging, telemetry and the UI
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Keeps the action context menu on screen after execution
        /// </summary>
        public bool KeepMenuShowingAfterExecution { get; set; }

        /// <summary>
        /// The publically exposed execution call
        /// </summary>
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

            LoggerEngineFactory.Debug(String.Format("Action: {0}, Entity: {1} [{2}]", Label, title, Entity.Kind));

            Statistics.Hit("Action." + Label);

            try
            {
                DoAction(Entity);
                if (ActionsModel.GetInstance.Visible)
                {
                    ActionsModel.GetInstance.Visible = KeepMenuShowingAfterExecution;
                }
            }
            catch(Exception e)
            {
                LoggerEngineFactory.Error(e);
            }

            if (Invoked != null)
            {
                Invoked(this, new EventArgs());
            }
        }

        /// <summary>
        /// Allows other code to watch for execution
        /// </summary>
        public event EventHandler Invoked;

        /// <summary>
        /// The payload of the call
        /// </summary>
        /// <param name="entity"></param>
        public abstract void DoAction(baseEntity entity);

        /// <summary>
        /// The path to the icon that will be used for the UI
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Returns the icon as an Image
        /// </summary>
        public Image Icon
        {
            get { return new Image(IconPath); }
        }

        /// <summary>
        /// Required to implement ICommand
        /// </summary>
        public bool Available { get; set; }

        public abstract baseActionCommand NewInstance(baseEntity entity);

        protected static bool InheritsFrom<T>(baseEntity e)
        {
            return typeof(T).IsAssignableFrom(e.GetType());
        }
    }
}
