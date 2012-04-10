using System.Collections.Generic;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class ViewMenuModel : BaseModel
    {
        #region singleton
        static ViewMenuModel _instance;
        static readonly object Lock = new object();

        public static ViewMenuModel GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
                {
                    return _instance ?? (_instance = new ViewMenuModel());
                }
            }
        }
        #endregion

        private bool _visible;
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
                return Engines.Themes.ThemeLoader.Views;
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
