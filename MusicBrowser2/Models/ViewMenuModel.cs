using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Models
{
    public class ViewMenuModel : BaseModel
    {
        #region singleton
        static ViewMenuModel _instance;
        static readonly object _lock = new object();

        public static ViewMenuModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ViewMenuModel();
                    }
                    return _instance;
                }
            }
        }
        #endregion

        public bool _visible = false;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    FirePropertyChanged("Visible");
                }
            }
        }
    }
}
