using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Models
{

    /// <summary>
    /// this is the model behind the search screen
    /// </summary>
    public class SearchModel : BaseModel
    {
        private enum SearchScope
        {
            Everything,
            Context,
            Albums,
            Artists,
            Tracks
        }

        private SearchScope _searchScope = SearchScope.Everything;


        public void SetSetSearchScope(string scope)
        {
            _searchScope = SearchScope.Everything;

            switch (scope.ToLower())
            {
                case "context":
                    _searchScope = SearchScope.Context;
                    break;
                case "albums":
                    _searchScope = SearchScope.Albums;
                    break;
                case "artists":
                    _searchScope = SearchScope.Artists;
                    break;
                case "tracks":
                    _searchScope = SearchScope.Tracks;
                    break;
            }
            FirePropertyChanged("Scope");
        }

        public string Scope
        {
            get
            {
                return _searchScope.ToString().ToLower();
            }
        }


        //private readonly EditableText _remoteFilter = new EditableText();
        //private int _matches;

            //        _matches = _fullentities.Count;

            //_remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;

                    
            //_fullentities = new EntityCollection();
            //_fullentities.AddRange(_entities);

        //void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        //{
        //    if (property == "Value")
        //    {
        //        Logging.Logger.Debug("filter = " + _remoteFilter.Value);

        //        // if it's resetting the filter, then just shortcut and load the
        //        // entity list with the full set of data
        //        if (string.IsNullOrEmpty(_remoteFilter.Value) || (_remoteFilter.Value.Contains('\\')))
        //        {
        //            RefreshEntities();
        //            return;
        //        }

        //        _matches = 0;
        //        Regex regex = new Regex("\\b" + _remoteFilter.Value.ToLower());
        //        EntityCollection temp = new EntityCollection();
        //        temp.AddRange(_fullentities.Where(item => regex.IsMatch(item.SortName)));

        //        _matches = temp.Count;
        //        if (_matches > 0)
        //        {
        //            temp.Sort();
        //            _entities.Clear();
        //            _entities.AddRange(temp);
        //        }
        //        else
        //        {
        //            _remoteFilter.Value = String.Empty;
        //        }
        //    }

        //    FirePropertyChanged("Matches");
        //    FirePropertyChanged("EntityList");
        //    FirePropertyChanged("FullSize");
        //    FirePropertyChanged("ShowFilterAsYouType");
        //}

        //public bool ShowFilterAsYouType
        //{
        //    get
        //    {
        //        if (_remoteFilter.Value == null) { return false; }
        //        return (_remoteFilter.Value.Trim().Length != 0);
        //    }
        //}

        //public EditableText RemoteTyper
        //{
        //    get { return _remoteFilter; }
        //}

        //public string Matches
        //{
        //    get { return _matches.ToString(); }
        //}

        //public string FullSize
        //{
        //    get
        //    {
        //        if (_matches != 0 & _matches != _fullentities.Count)
        //        {
        //            return "(" + _fullentities.Count + ")";
        //        }
        //        return string.Empty;
        //    }
        //}

        //private void RefreshEntities()
        //{
        //    _entities.Clear();
        //    _entities.AddRange(_fullentities);
        //    _matches = _entities.Count;
        //    _entities.Sort();
        //    _remoteFilter.Value = String.Empty;
        //    FirePropertyChanged("Matches");
        //    FirePropertyChanged("EntityList");
        //    FirePropertyChanged("FullSize");
        //}
    }
}
