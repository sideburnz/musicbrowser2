using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.MediaCentre;
using MusicBrowser.Models;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers;

// ReSharper disable CheckNamespace
namespace MusicBrowser
// ReSharper restore CheckNamespace
{
    public class Application : ModelItem
    {
        private readonly AddInHost _host;
        private readonly HistoryOrientedPageSession _session;

        private readonly IEntityFactory _factory;
        private readonly IEntityCache _cache;

        public Application() : this(null, null) { }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            _session = session; 
            _host = host;

            _cache = new EntityCache();
            _factory = new EntityFactory();
            _factory.setCache(_cache);
        }

        private MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (_host == null) return null;
                return _host.MediaCenterEnvironment;
            }
        }

        public void Navigate(IEntity entity, Breadcrumbs parentCrumbs)
        {
            Logging.Logger.Info("Navigating to " + entity.Description);
            try
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Application"] = this;

                if (entity.Kind == EntityKind.Song)
                {
                    //Breadcrumbs crumbs = new Breadcrumbs(parentCrumbs);
                    //crumbs.Add(entity);
                    properties["Song"] = new SongModel((Entities.Kinds.Song)entity);
                    properties["UINotifier"] = UINotifier.GetInstance();

                    //properties["Crumbs"] = crumbs;
                    _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageSong", properties);
                }
                else if (entity.Kind == EntityKind.Playlist)
                {
                    Playlist.PlaySong(entity, false);
                }
                else if (entity.Kind == EntityKind.Home)
                {
                    EntityCollection entities = new EntityCollection();
                    Entities.Kinds.Home home = (Entities.Kinds.Home)entity;
                    foreach (string item in home.Paths)
                    {
                        entities.Populate(FileSystemProvider.GetFolderContents(item), _factory, entity);
                    }
                    FolderModel folderModel = new FolderModel(entity, parentCrumbs, entities);
                    properties["FolderModel"] = folderModel;
                    properties["UINotifier"] = UINotifier.GetInstance();
                    _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);

                    //trigger the background caching tasks
                    foreach (string path in home.Paths)
                    {
                        CommonTaskQueue.Enqueue(new BackgroundCacheProvider(path, _factory, _cache, new Entities.Kinds.Unknown()));
                    }

                }
                else // assume it's some sort of folder
                {
                    EntityCollection entities = new EntityCollection();
                    entities.Populate(FileSystemProvider.GetFolderContents(entity.Path), _factory, entity);
                    FolderModel folderModel = new FolderModel(entity, parentCrumbs, entities);
                    properties["FolderModel"] = folderModel;
                    properties["UINotifier"] = UINotifier.GetInstance();
                    _session.GoToPage("resx://MusicBrowser/MusicBrowser.Resources/pageFolder", properties);
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Error(ex);
                DialogTest("Failed to navigate to " + entity.Description);
            }
        }

        private void DialogTest(string strClickedText)
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

