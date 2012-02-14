using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers.Background;
using MusicBrowser.Models.Keyboard;
using MusicBrowser.Engines;
using MusicBrowser.Engines.Virtuals;

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

            LoggerEngineFactory.Info("Starting MusicBrowser 2 - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

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

        public static string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void NavigateToFoo()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("Model", Foobar2000.GetInstance());
            _session.GoToPage(Engines.Themes.Theme.FooPlaying, props);
        }

        public void NavigateToSettings()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("VersionString", "Version: " + Application.Version);
            _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageSettings", props);
        }

        public void NavigateToSearch(string searchString, baseEntity entity)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("Model", new SearchModel(searchString));
            props.Add("ActionsModel", ActionsModel.GetInstance);
            props.Add("UINotifier", UINotifier.GetInstance());
            _session.GoToPage(Engines.Themes.Theme.Search, props);
        }

        public void Navigate(baseEntity entity)
        {
            LoggerEngineFactory.Info(String.Format("Navigating to {0} [{1}]", entity.Title, entity.Kind));
            try
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                EntityCollection entities = new EntityCollection();

                switch (entity.Kind)
                {
                    case "Home":
                        {
                            entities = HomeScreen.Entities;
                            foreach (baseEntity e in entities)
                            {
                                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(e));
                            }
                            break;
                        }
                    case "MusicCollection":
                    case "VideoCollection":
                    case "Collection":
                        {
                            IFolderItemsProvider fip = new CollectionProvider();
                            entities.AddRange(fip.GetItems(entity.Path));
                            break;
                        }
                    case "Virtual":
                        {
                            IView view = Views.Fetch(entity.Title);
                            entities = view.Items;
                            entity.SortField = view.Sort;
                            entity.SortAscending = view.SortAscending;
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
                FolderModel folderModel = new FolderModel(entity, entities, KeyboardHandlerFactory.GetHandler());
                folderModel.application = this;
                properties["FolderModel"] = folderModel;
                properties["UINotifier"] = UINotifier.GetInstance();
                properties["ActionsModel"] = ActionsModel.GetInstance;
                _session.GoToPage(Engines.Themes.Theme.Main, properties);
            }
            catch (Exception ex)
            {
                LoggerEngineFactory.Error(ex);
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

