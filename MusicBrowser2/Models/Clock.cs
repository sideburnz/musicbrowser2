using System;
using Microsoft.MediaCenter.UI;

//*****************************************************************************
// JJ - taken and updated from the MediaBrowser project
//*****************************************************************************

namespace MusicBrowser.Models
{
    public class Clock : ModelItem
    {
        private string _time = String.Empty;
        private readonly Timer _timer;
        private readonly string _timeformat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

        public Clock()
        {
            _timer = new Timer(this) {Interval = 10000};
            _timer.Tick += delegate { RefreshTime(); };
            _timer.Enabled = true;
            RefreshTime();
        }

        // Current time. 
        public string Time
        {
            get { return _time; }
            set
            {
                if (_time == value) return;
                _time = value;
                FirePropertyChanged("Time");
            }
        }

        // Try to update the time.
        private void RefreshTime()
        {
            Time = DateTime.Now.ToString(_timeformat);
        }
    }

}
