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
        private EntityKind _searchScope;
        private readonly InMemoryCache _fullCollection;
        private readonly EditableText _remoteFilter = new EditableText();



        public SearchModel()
        {
            _searchScope = EntityKind.Unknown;
            _fullCollection = InMemoryCache.GetInstance();

            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
        }

        public void SetSetSearchScope(string scope)
        {
            switch (scope.ToLower())
            {
                case "folder":
                    _searchScope = EntityKind.Folder;
                    break;
                case "albums":
                    _searchScope = EntityKind.Album;
                    break;
                case "artists":
                    _searchScope = EntityKind.Artist;
                    break;
                case "tracks":
                    _searchScope = EntityKind.Track;
                    break;
                default:
                    _searchScope = EntityKind.Unknown;
                    break;
            }
            FirePropertyChanged("Scope");
            FirePropertyChanged("ResultSet");
        }

        public string Scope
        {
            get
            {
                if (_searchScope == EntityKind.Folder)
                {
                    return "Current Folder";
                }
                if (_searchScope == EntityKind.Unknown)
                {
                    return "Everything";
                }
                return _searchScope.ToString() + "s";
            }
        }

        public VirtualList ResultSet
        {
            get
            {
                string filterText = String.Empty;
                if (_remoteFilter.Value != null)
                {
                    filterText = _remoteFilter.Value;
                }
                EntityCollection results = _fullCollection.DataSet.Filter(_searchScope, filterText);
                return new EntityVirtualList(results, false);
            }
        }

        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        {
            FirePropertyChanged("ResultSet");
        }

        public EditableText KeyboardHandler
        {
            get { return _remoteFilter; }
        }

    }
}
