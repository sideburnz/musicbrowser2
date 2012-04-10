using System.Collections.Generic;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using Factory = MusicBrowser.Actions.Factory;

namespace MusicBrowser.Models
{
    public class ActionsModel : BaseModel
    {
        #region singleton
        static ActionsModel _instance;
        static readonly object Lock = new object();
        
        public static ActionsModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
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
                FirePropertyChanged("Actions");
            }
        }

        public List<baseActionCommand> Actions
        {
            get
            {
                return Factory.GetActionList(_context);
            }
        }
    }
}
