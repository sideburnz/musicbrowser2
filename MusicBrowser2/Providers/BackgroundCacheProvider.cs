﻿using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Interfaces;
using System.Linq;

namespace MusicBrowser.Providers
{
    class BackgroundCacheProvider : IBackgroundTaskable 
    {
        private readonly string _path;
        private readonly EntityFactory _factory;

        public BackgroundCacheProvider(string path, EntityFactory factory)
        {
            _path = path;
            _factory = factory;
        }

        #region IBackgroundTaskable Members

        public string Title
        {
            get { return "BackgroundCacheProvider(" + _path + ")"; }
        }

        public void Execute()
        {
#if DEBUG
            Logging.Logger.Verbose(this.GetType().ToString(), "start");
#endif
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_path);
            IEnumerable<IDataProvider> providers = MetadataProviderList.GetProviders();

            foreach (FileSystemItem item in items)
            {
                // don't waste time on the item
                if (!Util.Helper.IsEntity(item.FullPath)) { continue; }
                if (item.Name.ToLower() == "metadata") { continue; }

                // process the item in context
                IEntity entity = _factory.GetItem(item);
                if (entity.Kind.Equals(EntityKind.Unknown) || entity.Kind.Equals(EntityKind.Folder)) { continue; }

                // fire off the metadata providers
                if (!entity.Kind.Equals(EntityKind.Home))
                {
                    CommonTaskQueue.Enqueue(new MetadataProviderList(entity, providers));
                }
            }
#if DEBUG
            Logging.Logger.Verbose(this.GetType().ToString() + " " + items.Count() , "end");
#endif
        }

        #endregion
    }
}
