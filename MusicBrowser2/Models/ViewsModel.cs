using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Views;
using MusicBrowser.Entities;
using MusicBrowser.Actions;

namespace MusicBrowser.Models
{
    public class ViewsModel : BaseModel
    {
        #region singleton
        static ViewsModel _instance;
        static readonly object _lock = new object();

        public static ViewsModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ViewsModel();
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
                    ActionOpenView action = new ActionOpenView(Context);
                    action.Title = view.Title;
                    commands.Add(action);
                }

                commands.Add(new ActionCloseMenu());

                return commands;
            }
        }
    }
}
