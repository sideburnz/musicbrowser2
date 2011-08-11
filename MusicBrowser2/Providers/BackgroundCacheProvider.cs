using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.Metadata;

namespace MusicBrowser.Providers
{
    class BackgroundCacheProvider : IBackgroundTaskable 
    {
        private readonly string _path;
        private readonly EntityFactory _factory;
        private readonly IEntity _entity;

        public BackgroundCacheProvider(string path, EntityFactory factory, IEntity entity)
        {
            _path = path;
            _factory = factory;
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
                // don't waste time on the item
                if (Util.Helper.IsNotEntity(item.FullPath)) { continue; }
                if (item.Name.ToLower() == "metadata") { continue; }

                // process the item in context
                IEntity entity = _factory.GetItem(item);

                if (entity.Kind.Equals(EntityKind.Unknown) || entity.Kind.Equals(EntityKind.Folder)) { continue; }

                entity.Parent = _entity;

                // fire off the metadata providers
                CommonTaskQueue.Enqueue(new MetadataProviderList(entity));

                // fire off cache tasks for sub items
                if (Util.Helper.IsFolder(item.Attributes))
                {
                    IBackgroundTaskable task = new BackgroundCacheProvider(item.FullPath, _factory, entity);
                    task.Execute();
                }

                entity.UpdateValues();
                CacheEngineFactory.GetCacheEngine().Update(entity.CacheKey, EntityPersistance.Serialize(entity));
            }

            _entity.UpdateValues();
            CacheEngineFactory.GetCacheEngine().Update(_entity.CacheKey, EntityPersistance.Serialize(_entity));
        }

        #endregion
    }
}
