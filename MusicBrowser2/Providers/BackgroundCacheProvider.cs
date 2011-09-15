using System.Collections.Generic;
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

        public BackgroundCacheProvider(string path)
        {
            _path = path;
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
                Entity entity = EntityFactory.GetItem(item);
                if (entity == null || entity.Kind.Equals(EntityKind.Folder)) { continue; }

                // fire off the metadata providers
                if (!entity.Kind.Equals(EntityKind.Home))
                {
                    CommonTaskQueue.Enqueue(new MetadataProviderList(entity));
                }
            }
#if DEBUG
            Logging.Logger.Verbose(this.GetType().ToString() + " " + items.Count() , "end");
#endif
        }

        #endregion
    }
}
