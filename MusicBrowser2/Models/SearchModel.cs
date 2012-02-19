using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Providers;

namespace MusicBrowser.Models
{

    /// <summary>
    /// this is the model behind the search screen
    /// </summary>
    public class SearchModel : BaseModel
    {
        private string _searchScope;
        private readonly EditableText _remoteFilter = new EditableText();
        private readonly ICacheEngine _engine = CacheEngineFactory.GetEngine();
        private Dictionary<string, int> _hitsByType = new Dictionary<string, int>();

        public SearchModel(string initialSearchString)
        {            
            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
            _remoteFilter.Value = initialSearchString;
            _searchScope = "Album";
        }

        public void TypeSpace()
        {
            _remoteFilter.Value += " ";
        }

        public void SetSetSearchScope(string scope)
        {
            _searchScope = scope;
            FirePropertyChanged("Scope");
            FirePropertyChanged("ResultSet");
        }

        public string SearchText
        {
            get
            {
                if (_remoteFilter.Value == null)
                {
                    return String.Empty;
                }
                return _remoteFilter.Value;
            }
        }

        public string Scope
        {
            get
            {
                return _searchScope.ToString() + "s";
            }
        }

        public VirtualList ResultSet
        {
            get
            {
                string value;
                if (!String.IsNullOrEmpty(_remoteFilter.Value))
                {
                    value = _remoteFilter.Value;
                    _hitsByType = _engine.HitsByType(value);
                }
                else
                {
                    value = String.Empty;
                    _hitsByType = _engine.HitsByType(String.Empty);
                }

                EntityCollection dataset = new EntityCollection();
                IEnumerable<string> searchResults = _engine.Search(_searchScope, value);
                foreach (string item in searchResults)
                {
                    dataset.Add(_engine.Fetch(item));
                }

                return new EntityVirtualList(dataset, "[Title]", true);
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

        public string ArtistsLabel
        {
            get { return FormatLabel("Artist"); }
        }

        public string AlbumsLabel
        {
            get { return FormatLabel("Album"); }
        }

        public string TracksLabel
        {
            get { return FormatLabel("Track"); }
        }

        public string MoviesLabel
        {
            get { return FormatLabel("Movie"); }
        }

        public string ShowsLabel
        {
            get { return FormatLabel("Show"); }
        }

        public string EpisodesLabel
        {
            get { return FormatLabel("Episode") ; }
        }

        private string FormatLabel(string type)
        {
            if (_hitsByType.ContainsKey(type))
            {
               return String.Format("{0}s ({1})", type, _hitsByType[type]);
            }
            return String.Empty;
        }

    }
}
