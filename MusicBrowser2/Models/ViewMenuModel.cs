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
                return new List<string>{ "Thumb", "List", "Strip" };
            }
        }

        public void DecreaseThumb()
        {
            int thumbSize = Entity.ViewState.ThumbSize;
            thumbSize -= 10;
            if (thumbSize < 80)
            {
                thumbSize = 80;
            }
            Entity.ViewState.SetThumbSize(thumbSize);
        }

        public void IncreaseThumb()
        {
            int thumbSize = Entity.ViewState.ThumbSize;
            thumbSize += 10;
            if (thumbSize > 350)
            {
                thumbSize = 350;
            }
            Entity.ViewState.SetThumbSize(thumbSize);
        }

        public void CommitChanges()
        {
            Entity.UpdateCache();
        }
    }
}
