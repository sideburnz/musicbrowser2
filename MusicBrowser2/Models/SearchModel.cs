//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.MediaCenter.UI;
//using MusicBrowser.Entities;
//using MusicBrowser.Engines.Cache;
//using MusicBrowser.Providers;

//namespace MusicBrowser.Models
//{

//    /// <summary>
//    /// this is the model behind the search screen
//    /// </summary>
//    public class SearchModel : BaseModel
//    {
//        private EntityKind _searchScope;
//        private readonly InMemoryCache _fullCollection;
//        private readonly EntityCollection _contextCollection;
//        private readonly EditableText _remoteFilter = new EditableText();

//        public SearchModel(string initialSearchString, baseEntity context)
//        {
//            _fullCollection = InMemoryCache.GetInstance();

//            if (context == null || context.Kind == EntityKind.Home || context.Kind == EntityKind.Group)
//            {
//                _searchScope = EntityKind.None;
//                HasContext = false;
//            }
//            else if (context.Kind == EntityKind.Virtual)
//            {
//                switch (context.Path.ToLower())
//                {
//                    //TODO: redo this group by logic 
//                    case "tracks by genre":
//                        {
//                            _contextCollection = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track, "Genre", context.Title);
//                            break;
//                        }
//                    case "albums by year":
//                        {
//                            _contextCollection = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "Year", context.Title);
//                            break;
//                        }
//                    case "albums":
//                        {
//                            _contextCollection = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "", context.Title);
//                            break;
//                        }
//                }
//                _searchScope = EntityKind.Folder;
//                HasContext = true;
//                ContextName = context.Title;
//            }
//            else 
//            {
//                _searchScope = EntityKind.Folder;
//                _contextCollection = new EntityCollection();
//                _contextCollection.AddRange(FileSystemProvider.GetAllSubPaths(context.Path)); //this line needs to change
//                HasContext = true;
//                ContextName = context.Title;
//            }
//            _remoteFilter.PropertyChanged += RemoteFilterPropertyChanged;
//            _remoteFilter.Value = initialSearchString;
//        }

//        public bool HasContext { get; set; }

//        public void TypeSpace()
//        {
//            _remoteFilter.Value += " ";
//        }

//        public void SetSetSearchScope(string scope)
//        {
//            switch (scope.ToLower())
//            {
//                case "folder":
//                    _searchScope = EntityKind.Folder;
//                    break;
//                case "albums":
//                    _searchScope = EntityKind.Album;
//                    break;
//                case "artists":
//                    _searchScope = EntityKind.Artist;
//                    break;
//                case "tracks":
//                    _searchScope = EntityKind.Track;
//                    break;
//                default:
//                    _searchScope = EntityKind.None;
//                    break;
//            }
//            FirePropertyChanged("Scope");
//            FirePropertyChanged("ResultSet");
//        }

//        public string ContextName { get; set; }

//        public string Scope
//        {
//            get
//            {
//                if (_searchScope == EntityKind.Folder)
//                {
//                    return "Current Folder";
//                }
//                if (_searchScope == EntityKind.None)
//                {
//                    return "Everything";
//                }
//                return _searchScope.ToString() + "s";
//            }
//        }

//        public VirtualList ResultSet
//        {
//            get
//            {
//                EntityCollection dataset;
               
//                if (HasContext && _searchScope == EntityKind.Folder)
//                {
//                    dataset = _contextCollection;
//                }
//                else
//                {
//                    dataset = _fullCollection.DataSet;
//                }

//                string filterText = String.Empty;
//                if (_remoteFilter.Value != null)
//                {
//                    filterText = _remoteFilter.Value;
//                }
//                EntityCollection results = dataset.Filter(_searchScope, filterText);
//                return new EntityVirtualList(results, false);
//            }
//        }

//        void RemoteFilterPropertyChanged(IPropertyObject sender, string property)
//        {
//            FirePropertyChanged("ResultSet");
//        }

//        public EditableText KeyboardHandler
//        {
//            get { return _remoteFilter; }
//        }

//    }
//}
