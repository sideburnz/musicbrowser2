using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
using MusicBrowser.Entities;

namespace MusicBrowser.Providers
{
    class ForceMetadataRefreshProvider : IBackgroundTaskable
    {
        private baseEntity _parent;

        public ForceMetadataRefreshProvider(baseEntity parent)
        {
            _parent = parent;
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
            //new Metadata.MetadataProviderList(_parent, true).Execute();

            // refresh the children item
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_parent.Path);
            foreach (FileSystemItem item in items)
            {
                baseEntity e = EntityFactory.GetItem(item);
                if (e == null) { continue; }
                Engines.Cache.InMemoryCache.GetInstance().Remove(e.CacheKey);
                Engines.Cache.CacheEngineFactory.GetEngine().Delete(e.CacheKey);
                //new Metadata.MetadataProviderList(e, true).Execute();
            }
        }
    }
}
