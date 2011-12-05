using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Actions;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class ActionsModel : BaseModel
    {
        #region singleton
        static ActionsModel _instance;
        static readonly object _lock = new object();
        
        public static ActionsModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ActionsModel();
                    }
                    return _instance;
                }
            }
        }
        #endregion

        public bool _visible = false;
        public baseEntity _context;

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

        public baseEntity Context 
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
                FirePropertyChanged("Context");
            }
        }
    }
}
