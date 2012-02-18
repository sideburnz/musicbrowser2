using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Virtuals;
using MusicBrowser.Entities;
using MusicBrowser.Actions;

namespace MusicBrowser.Models
{
    public class VirtualsModel : BaseModel
    {
        #region singleton
        static VirtualsModel _instance;
        static readonly object _lock = new object();

        public static VirtualsModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new VirtualsModel();
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
                FirePropertyChanged("Virtuals");
            }
        }

        public List<baseActionCommand> Virtuals
        {
            get
            {
                List<baseActionCommand> commands = new List<baseActionCommand>();

                foreach(IView view in Views.GetViews(Context.Kind))
                {
                    ActionOpenVirtual action = new ActionOpenVirtual(Context);
                    action.Title = view.Title;
                    commands.Add(action);
                }

                commands.Add(new ActionCloseMenu());

                return commands;
            }
        }
    }
}
