using System;
using MusicBrowser.Util;
using MusicBrowser.Models;

namespace MusicBrowser.Engines.ViewState
{
    public abstract class BaseViewState : BaseModel, IViewState
    {
        private static readonly Config Config = Config.GetInstance();


        private string _view = String.Empty;
        private string _sortField = String.Empty;
        private readonly string _key;
        private bool _sortAsc = true; // 

        protected BaseViewState(string key)
        {
            ThumbSize = Config.GetIntSetting("Views.ThumbSize");
            _key = key;
        }

        protected string key
        {
            get { return _key; }
        }

        public string DefaultSort { get; set; }

        public string DefaultView { get; set; }

        public int ThumbSize { get; protected set; }

        // not an auto property because it needs to be defaulted to true
        public virtual bool SortAscending 
        {
            get { return _sortAsc; }
            protected set { _sortAsc = value; } 
        }


        public virtual String View
        {
            get
            {
                // if view is overriden for this single entity, use its setting
                if (!String.IsNullOrEmpty(_view))
                {
                    return _view;
                }
                if (!String.IsNullOrEmpty(DefaultView))
                {
                    return DefaultView;
                }
                return "List";
            }
            protected set
            {
                _view = value;
            }
        }

        public virtual String SortField
        {
            get
            {
                if (!String.IsNullOrEmpty(_sortField))
                {
                    return _sortField;
                }
                if (!String.IsNullOrEmpty(DefaultSort))
                {
                    return DefaultSort;
                }
                return "[Title:sort]";
            }
            protected set
            {
                _sortField = value;
            }
        }


        public virtual void SetThumbSize(int size)
        {
            if (ThumbSize != size)
            {
                ThumbSize = size;
                FirePropertyChanged("ThumbSize");
            }
        }

        public virtual void SetSortField(string field)
        {
            if (!field.StartsWith("["))
            {
                field = "[" + field + ":sort]";
            }
            if (_sortField != field)
            {
                _sortField = field;
                FirePropertyChanged("SortField");
            }
        }

        public virtual void InvertSort()
        {
            SortAscending = !SortAscending;
            FirePropertyChanged("SortAscending");
        }

        public virtual void SetView(string view)
        {
            if (_view != view)
            {
                _view = view;
                FirePropertyChanged("View");
            }
        }
    }
}