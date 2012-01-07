using System;
using System.Collections.Generic;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;

namespace MusicBrowser.Providers
{
    class BackgroundCacheProvider : IBackgroundTaskable
    {
        private readonly string _path;

        public BackgroundCacheProvider(string path)
        {
            _path = path;
        }

        public BackgroundCacheProvider(baseEntity e)
        {
            IFolderItemsProvider fip = new CollectionProvider();
            IEnumerable<string> paths = fip.GetItems(e.Path);
            foreach (string path in paths)
            {
                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(path));
            }
            _path = String.Empty;
        }

        #region IBackgroundTaskable Members

        public string Title
        {
            get { return "BackgroundCacheProvider(" + _path + ")"; }
        }

        public void Execute()
        {
            if (String.IsNullOrEmpty(_path)) { return; }

#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(this.GetType().ToString(), "start");
#endif
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_path);
            //IEnumerable<IDataProvider> providers = MetadataProviderList.GetProviders();

            foreach (FileSystemItem item in items)
            {
                try
                {
                    // process the item in context
                    baseEntity entity = EntityFactory.GetItem(item);
                    if (entity == null) { continue; }
                    // fire off the metadata providers
                    //CommonTaskQueue.Enqueue(new MetadataProviderList(entity));
                    entity.UpdateCache();
                }
                catch (Exception e)
                {
                    LoggerEngineFactory.Error(e);
                }
            }
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(this.GetType().ToString(), "end");
#endif
        }

        #endregion
    }
}
