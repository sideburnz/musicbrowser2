using System.Collections.Generic;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Util;

namespace MusicBrowser.Providers
{
    class ForceMetadataRefreshProvider : IBackgroundTaskable
    {
        private readonly baseEntity _parent;
        private readonly bool _recurse;

        public ForceMetadataRefreshProvider(baseEntity parent, bool recurse)
        {
            _parent = parent;
            _recurse = recurse;
        }

        public string Title
        {
            get { return "ForceMetadataRefreshProvider(" + _parent.Path +")"; }
        }

        public void Execute()
        {
            // refresh the current item
            Engines.Cache.InMemoryCache.GetInstance().Remove(_parent.CacheKey);
            Engines.Cache.CacheEngineFactory.GetEngine().Delete(_parent.CacheKey);
            new Engines.Metadata.MetadataProviderList(_parent, true).Execute();

            if (_recurse)
            {
                // refresh the children item
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_parent.Path).FilterInternalFiles();
                foreach (FileSystemItem item in items)
                {
                    string key = Helper.GetCacheKey(item.FullPath);

                    Engines.Cache.InMemoryCache.GetInstance().Remove(key);
                    Engines.Cache.CacheEngineFactory.GetEngine().Delete(key);

                    baseEntity e = EntityFactory.GetItem(item);
                    if (e == null) { continue; }
                    
                    new Engines.Metadata.MetadataProviderList(e, true).Execute();
                }
            }
        }
    }
}
