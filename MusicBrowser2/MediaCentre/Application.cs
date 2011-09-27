using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.MediaCentre;
using MusicBrowser.Models;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;

// ReSharper disable CheckNamespace
namespace MusicBrowser
// ReSharper restore CheckNamespace
{
    public class Application
    {
        private readonly AddInHost _host;
        private readonly HistoryOrientedPageSession _session;

        public Application() : this(null, null) { }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            _session = session; 
            _host = host;

            Util.Config.GetInstance().SetDefaultSettings();
            Logging.Logger.Info("Starting MusicBrowser 2 - " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }

        private MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (_host == null) return null;
                return _host.MediaCenterEnvironment;
            }
        }

        public void Navigate(Entity entity, Breadcrumbs parentCrumbs)
        {
            Logging.Logger.Info("Navigating to " + entity.Description);
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
                                entities.Populate(FileSystemProvider.GetFolderContents(item));
                            }
                            break;
                        }
                    case EntityKind.Group:
                        {
                            switch (entity.Title.ToLower())
                            {
                                case "tracks by genre":
                                    {
                                        entities = NearLineCache.GetInstance().DataSet.Group(EntityKind.Track, "genre");
                                        break;
                                    }
                                case "albums by year":
                                    {
                                        entities = NearLineCache.GetInstance().DataSet.Group(EntityKind.Album, "year");
                                        break;
                                    }
                            }
                            break;
                        }
                    case EntityKind.Virtual:
                        {
                            switch (entity.Path)
                            {
                                case "tracks by genre":
                                    {
                                        entities = NearLineCache.GetInstance().DataSet.Filter(EntityKind.Track, "genre", entity.Title);
                                        break;
                                    }
                                case "albums by year":
                                    {
                                        entities = NearLineCache.GetInstance().DataSet.Filter(EntityKind.Album, "year", entity.Title);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            entities.Populate(FileSystemProvider.GetFolderContents(entity.Path));
                            break;
                        }
                }

                if (entities.Count == 0)
                {
                    Dialog("The selected item is empty");
                    return;
                }

                entities.Sort();

                properties["Application"] = this;
                FolderModel folderModel = new FolderModel(entity, parentCrumbs, entities);
                folderModel.application = this;
                properties["FolderModel"] = folderModel;
                properties["UINotifier"] = UINotifier.GetInstance();
                _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);
            }
            catch (Exception ex)
            {
                Logging.Logger.Error(ex);
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

