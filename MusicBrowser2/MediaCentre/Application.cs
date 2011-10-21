using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;

// ReSharper disable CheckNamespace
namespace MusicBrowser
// ReSharper restore CheckNamespace
{
    public class Application
    {
        private readonly AddInHost _host;
        private readonly HistoryOrientedPageSession _session;
        private static Application _application;

        public Application() : this(null, null) { }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            _session = session; 
            _host = host;

            Util.Config.GetInstance().SetDefaultSettings();
            Logger.Info("Starting MusicBrowser 2 - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

            _application = this;
        }

        public static Application GetReference()
        {
            return _application;
        }

        public HistoryOrientedPageSession Session()
        {
            return _session;
        }

        private MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (_host == null) return null;
                return _host.MediaCenterEnvironment;
            }
        }

        public void NavigateToSettings()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("Model", new ConfigModel());
            _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageSettings", props);
        }

        public void NavigateToSearch(string searchString, Entity entity)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("Model", new SearchModel(searchString, entity));
            _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageSearch", props);
        }

        public void Navigate(Entity entity)
        {
            Logger.Info("Navigating to " + entity.Description);
            try
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                EntityCollection entities = new EntityCollection();

                switch (entity.Kind)
                {
                    case EntityKind.Home:
                        {
                            foreach (string item in Providers.FolderItems.HomePathProvider.Paths)
                            {
                                entities.AddRange(FileSystemProvider.GetFolderContents(item));
                            }
                            break;
                        }
                    case EntityKind.Group:
                        {
                            switch (entity.Title.ToLower())
                            {
                                case "tracks by genre":
                                    {
                                        entities = InMemoryCache.GetInstance().DataSet.Group(EntityKind.Track, "Genre");
                                        break;
                                    }
                                case "albums by year":
                                    {
                                        entities = InMemoryCache.GetInstance().DataSet.Group(EntityKind.Album, "Year");
                                        break;
                                    }
                            }
                            break;
                        }
                    case EntityKind.Virtual:
                        {
                            switch (entity.Path.ToLower())
                            {
                                case "tracks by genre":
                                    {
                                        entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track, "Genre", entity.Title);
                                        break;
                                    }
                                case "albums by year":
                                    {
                                        entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "Year", entity.Title);
                                        break;
                                    }
                                case "albums":
                                    {
                                        entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "", entity.Title);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            entities.AddRange(FileSystemProvider.GetFolderContents(entity.Path));
                            break;
                        }
                }

                if (entities.Count == 0)
                {
                    Dialog("The selected item is empty");
                    return;
                }

                properties["Application"] = this;
                FolderModel folderModel = new FolderModel(entity, entities);
                folderModel.application = this;
                properties["FolderModel"] = folderModel;
                properties["UINotifier"] = UINotifier.GetInstance();
                properties["ActionsModel"] = ActionsModel.GetInstance;
                _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Dialog("Failed to navigate to " + entity.Description);
            }
        }

        private void Dialog(string strClickedText)
        {
            const int timeout = 5;
            const bool modal = true;
            if (_session != null)
            {
                MediaCenterEnvironment.Dialog(strClickedText,
                    "MusicBrowser 2",
                    new object[] { DialogButtons.Ok },
                    timeout,
                    modal,
                    null,
                    delegate { });
            }
        }
    } 
} 

