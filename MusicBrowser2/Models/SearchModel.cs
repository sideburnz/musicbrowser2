﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.CacheEngine;
using MusicBrowser.Providers;

namespace MusicBrowser.Models
{

    /// <summary>
    /// this is the model behind the search screen
    /// </summary>
    public class SearchModel : BaseModel
    {
        private EntityKind _searchScope;
        private readonly InMemoryCache _fullCollection;
        private readonly EntityCollection _contextCollection;
        private readonly EditableText _remoteFilter = new EditableText();

        public SearchModel(string initialSearchString, Entity context)
        {
            _fullCollection = InMemoryCache.GetInstance();

            if (context == null || context.Kind == EntityKind.Home)
            {
                _searchScope = EntityKind.None;
                HasContext = false;
            }
            else //TODO: support for genres etc
            {
                _searchScope = EntityKind.Folder;
                _contextCollection = new EntityCollection();
                _contextCollection.AddRange(FileSystemProvider.GetAllSubPaths(context.Path)); //this line needs to change
                HasContext = true;
            }
            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
            _remoteFilter.Value = initialSearchString;
        }

        public bool HasContext { get; set; }

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
                    _searchScope = EntityKind.None;
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
                if (_searchScope == EntityKind.None)
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
                EntityCollection dataset;
               
                if (HasContext && _searchScope == EntityKind.Folder)
                {
                    dataset = _contextCollection;
                }
                else
                {
                    dataset = _fullCollection.DataSet;
                }

                string filterText = String.Empty;
                if (_remoteFilter.Value != null)
                {
                    filterText = _remoteFilter.Value;
                }
                EntityCollection results = dataset.Filter(_searchScope, filterText);
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
