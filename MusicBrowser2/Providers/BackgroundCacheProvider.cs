using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Entities.Interfaces;

namespace MusicBrowser.Providers
{
    class BackgroundCacheProvider : IBackgroundTaskable 
    {
        private readonly string _path;
        private readonly IEntityFactory _factory;
        private readonly IEntityCache _cache;
        private readonly IEntity _entity;

        public BackgroundCacheProvider(string path, IEntityFactory factory, IEntityCache cache, IEntity entity)
        {
            _path = path;
            _factory = factory;
            _cache = cache;
            _entity = entity;
        }

        #region IBackgroundTaskable Members

        public string Title
        {
            get { return "BackgroundCacheProvider(" + _path + ")"; }
        }

        public void Execute()
        {
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(_path);
            foreach (FileSystemItem item in items)
            {
                // process the item in context
                IEntity entity = _factory.getItem(item);
                entity.Parent = _entity;

                if (!entity.Kind.Equals(EntityKind.Unknown))
                {
                    // fire off the metadata providers
                    CommonTaskQueue.Enqueue(new MetadataProviderList(entity, _cache));
                }

                // fire off cache tasks for sub items
                if (Util.Helper.IsFolder(item.Attributes))
                {
                    CommonTaskQueue.Enqueue(new BackgroundCacheProvider(item.FullPath, _factory, _cache, entity));
                }
            }
        }

        #endregion
    }
}
