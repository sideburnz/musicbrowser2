using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;

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

            int count = 0;
            long grandchildren = 0;
            long duration = 0;

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

                count++;
                if (entity.Duration > 0) { duration += entity.Duration; }
                if (entity.Children > 0) { grandchildren += entity.Children; }
            }

            if (_entity.Children == 0) { _entity.Children = count; }
            if (_entity.Duration == 0) { _entity.Duration = duration; }
            if ((grandchildren > 0) && (_entity.Kind.Equals(EntityKind.Artist))) { ((Artist)_entity).GrandChildren = grandchildren; }

            _entity.CalculateValues();
        }

        #endregion
    }
}
