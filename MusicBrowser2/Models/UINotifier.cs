using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Models
{
    /// <summary>
    /// This class provides the UI interaction for the playlist
    /// </summary>
    public class UINotifier : Models.BaseModel
    {
        #region singleton
        private static UINotifier _instance = null;
        private static readonly object Padlock = new object();

        public static UINotifier GetInstance()
        {
            lock (Padlock)
            {
                if (_instance != null) return _instance;
                _instance = new UINotifier();
                return _instance;
            }
        }
        #endregion

        /**********************************************************************
         * TODO: make configurable
         * *******************************************************************/

        private string _message = "no message";
        private readonly System.Timers.Timer _timer;
        private bool _active = false;

        private UINotifier()
        {
            _timer = new System.Timers.Timer { Interval = 5000 };
            _timer.Elapsed += delegate { TurnOffNotice(); };
            _timer.Enabled = false;
        }

        public string Message 
        {
            get { return " " + _message + " "; } 
            set 
            {
                _message = value;
                if (!String.IsNullOrEmpty(value)) 
                {
                    _active = true;
                    _timer.Start();
                }
                FirePropertiesChanged("Message", "ShowPopUp");
            } 
        }

        public bool ShowPopUp
        {
            get { return _active; }
        }

        private void TurnOffNotice()
        {
            _timer.Stop();
            _active = false;
            FirePropertiesChanged("ShowPopUp");
        }

    }
}
