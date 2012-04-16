using System;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;

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
        protected baseActionCommand()
        {
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/nullImage";
            Label = GetType().Name;
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
            string kind;
            if (Entity == null)
            {
                title = "none defined";
                kind = "null";
            }
            else
            {
                title = Entity.Title;
                kind = Entity.Kind;
            }

            LoggerEngineFactory.Debug("baseActionCommand", String.Format("Action: {0}, Entity: {1} [{2}] ({3})", Label, title, kind, Available));

            // if the action isn't enabled, do run it
            if (!Available)
            {
                return;
            }

            Telemetry.Hit("Action." + Label.Replace(" ", ""));

            try
            {
                DoAction(Entity);
                if (ActionsModel.GetInstance.Visible)
                {
                    ActionsModel.GetInstance.Visible = KeepMenuShowingAfterExecution;
                }
                if (ViewsModel.GetInstance.Visible)
                {
                    ViewsModel.GetInstance.Visible = KeepMenuShowingAfterExecution;
                }
                if (ViewMenuModel.GetInstance.Visible)
                {
                    ViewMenuModel.GetInstance.Visible = KeepMenuShowingAfterExecution;
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
            FirePropertyChanged("Invoked");
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
        private string _iconPath = String.Empty;
        public string IconPath 
        {
            get { return _iconPath; }
            set { _iconPath = value; FirePropertyChanged("Icon"); } 
        }

        private Single _alpha = 1;
        public Single Alpha
        {
            get { return _alpha; }
            set { _alpha = value; FirePropertyChanged("Alpha"); }
        }

        /// <summary>
        /// Returns the icon as an Image
        /// </summary>
        public Image Icon
        {
            get 
            {
                string path = IconPath;
                if (!path.StartsWith("resx://"))
                {
                    path = "resx://" + path;
                }
                return new Image(path); 
            }
        }

        /// <summary>
        /// Required to implement ICommand
        /// </summary>
        public bool Available { get; set; }

        public abstract baseActionCommand NewInstance(baseEntity entity);

        protected static bool InheritsFrom<T>(baseEntity e)
        {
            return e is T;
        }
    }
}
