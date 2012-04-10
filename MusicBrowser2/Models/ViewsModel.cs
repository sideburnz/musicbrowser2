using System.Collections.Generic;
using System.Linq;
using MusicBrowser.Actions;
using MusicBrowser.Engines.Views;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class ViewsModel : BaseModel
    {
        #region singleton
        static ViewsModel _instance;
        static readonly object Lock = new object();

        public static ViewsModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
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

        private bool _visible;
        private baseEntity _context;

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
                List<baseActionCommand> commands = Views.GetViews(Context.Kind).Select(view => new ActionOpenView(Context) {Title = view.Title}).Cast<baseActionCommand>().ToList();
                commands.Add(new ActionCloseMenu());
                return commands;
            }
        }
    }
}
