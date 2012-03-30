using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Engines.Metadata;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Util;

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
            LoggerEngineFactory.Verbose(this.GetType().ToString(), "start");
#endif

            // process the item in context
            baseEntity e = EntityFactory.GetItem(_path);
            if (e != null)
            {
                // fire off the metadata providers
                CommonTaskQueue.Enqueue(new MetadataProviderList(e));
            }

            // don't waste time doing anything else if the current item isn't a folder
            if (!Directory.Exists(_path)) { return; }

            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_path).FilterInternalFiles();
            //IEnumerable<IDataProvider> providers = MetadataProviderList.GetProviders();

            foreach (FileSystemItem item in items)
            {
                try
                {
                    // process the item in context
                    baseEntity entity = EntityFactory.GetItem(item);
                    if (entity == null) { continue; }
                    // fire off the metadata providers
                    CommonTaskQueue.Enqueue(new MetadataProviderList(entity));
                }
                catch (Exception ex)
                {
                    LoggerEngineFactory.Error(ex);
                }
            }
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(this.GetType().ToString(), "end");
#endif
        }

        #endregion
    }
}
