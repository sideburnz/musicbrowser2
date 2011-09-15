﻿using System;
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
                properties["Application"] = this;

                switch (entity.Kind)
                {
                    case EntityKind.Home:
                        {
                            EntityCollection entities = new EntityCollection();
                            foreach (string item in Providers.FolderItems.HomePathProvider.Paths)
                            {
                                entities.Populate(FileSystemProvider.GetFolderContents(item));
                            }

                            FolderModel folderModel = new FolderModel(entity, parentCrumbs, entities);
                            properties["FolderModel"] = folderModel;
                            properties["UINotifier"] = UINotifier.GetInstance();
                            _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);

                            break;
                        }
                    default:
                        {
                            EntityCollection entities = new EntityCollection();
                            entities.Populate(FileSystemProvider.GetFolderContents(entity.Path));
                            FolderModel folderModel = new FolderModel(entity, parentCrumbs, entities);
                            properties["FolderModel"] = folderModel;
                            properties["UINotifier"] = UINotifier.GetInstance();
                            _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);
                            break;
                        }
                }
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

