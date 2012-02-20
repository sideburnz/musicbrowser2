using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

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

        public baseEntity Entity { get; set; }

        public List<string> Views
        {
            get
            {
                return Engines.Themes.Theme.Views;
            }
        }

        public void DecreaseThumb()
        {
            Entity.ThumbSize -= 10;
            if (Entity.ThumbSize < 80)
            {
                Entity.ThumbSize = 80;
            }
            Entity.UpdateCache();
        }

        public void IncreaseThumb()
        {
            Entity.ThumbSize += 10;
            if (Entity.ThumbSize > 300)
            {
                Entity.ThumbSize = 300;
            }
            Entity.UpdateCache();
        }

        public void CommitChanges()
        {
            Entity.UpdateCache();
        }
    }
}
