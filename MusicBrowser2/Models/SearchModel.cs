using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.CacheEngine;

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
            Folder,
            Albums,
            Artists,
            Tracks
        }

        private SearchScope _searchScope;
        private readonly InMemoryCache _fullCollection;
        private EntityCollection _searchCollection;

        private readonly EditableText _remoteFilter = new EditableText();


        public SearchModel()
        {
            _searchScope = SearchScope.Everything;
            _fullCollection = InMemoryCache.GetInstance();
            _searchCollection = _fullCollection.DataSet;

            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
        }

        public void SetSetSearchScope(string scope)
        {
            switch (scope.ToLower())
            {
                case "folder":
                    _searchScope = SearchScope.Folder;
                    break;
                case "albums":
                    _searchScope = SearchScope.Albums;
                    _searchCollection = _fullCollection.DataSet.Filter(EntityKind.Album, "");
                    break;
                case "artists":
                    _searchScope = SearchScope.Artists;
                    _searchCollection = _fullCollection.DataSet.Filter(EntityKind.Artist, "");
                    break;
                case "tracks":
                    _searchScope = SearchScope.Tracks;
                    _searchCollection = _fullCollection.DataSet.Filter(EntityKind.Track, "");
                    break;
                default:
                    _searchScope = SearchScope.Everything;
                    _searchCollection = _fullCollection.DataSet;
                    break;
            }
            FirePropertyChanged("Scope");
            FirePropertyChanged("ResultSet");
        }

        public string Scope
        {
            get
            {
                if (_searchScope == SearchScope.Folder)
                {
                    return "Current Folder";
                }
                return _searchScope.ToString();
            }
        }

        public VirtualList ResultSet
        {
            get
            {
                EntityCollection results = _searchCollection;
                //TODO: apply filter
                return new EntityVirtualList(results, false);
            }
        }

        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        {
            if (property == "Value")
            {
//                _remoteFilter.Value
            }
        }

        public EditableText KeyboardHandler
        {
            get { return _remoteFilter; }
        }

    }
}
