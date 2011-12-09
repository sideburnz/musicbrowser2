﻿using System;
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
                EntityCollection dataset = new EntityCollection();

                IEnumerable<string> searchResults = _engine.Search(_searchScope, _remoteFilter.Value);

                foreach (string item in searchResults)
                {
                    dataset.Add(_engine.Fetch(item));
                }

                return new EntityVirtualList(dataset, false);
            }
        }

        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
        {
            FirePropertyChanged("ResultSet");
            FirePropertyChanged("MatchedArtists");
        }

        public EditableText KeyboardHandler
        {
            get { return _remoteFilter; }
        }

        public string ArtistsLabel
        {
            get 
            {
                string r = FormatLabel("Artist");
                if (String.IsNullOrEmpty(r))
                {
                //    SetSetSearchScope("Album");
                }
                return r; 
            }
        }

        public string AlbumsLabel
        {
            get 
            {
                string r = FormatLabel("Album");
                if (String.IsNullOrEmpty(r))
                {
                //    SetSetSearchScope("Track");
                }
                return r; 
            }
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
            int hits = _engine.Search(type, _remoteFilter.Value).Count();
            if (hits > 0)
            {
                return String.Format("{0}s ({1})", type, hits);
            }
            return String.Empty;
        }

    }
}
