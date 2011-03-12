using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata;
using MusicBrowser.Entities.Interfaces;
using MusicBrowser.Entities.Kinds;

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

            int count = 0;
            long grandchildren = 0;
            long duration = 0;

            foreach (FileSystemItem item in items)
            {
                // don't waste time on the item
                if (Util.Helper.IsNotEntity(item.FullPath)) { continue; }
                if (item.Name.ToLower() == "metadata") { continue; }

                // process the item in context
                IEntity entity = _factory.getItem(item);
                entity.Parent = _entity;

                if (!entity.Kind.Equals(EntityKind.Unknown))
                {
                    // fire off the metadata providers
                    CommonTaskQueue.Enqueue(new MetadataProviderList(entity, _cache));
                    count++;
                }

                // fire off cache tasks for sub items
                if (Util.Helper.IsFolder(item.Attributes))
                {
                    IBackgroundTaskable task = new BackgroundCacheProvider(item.FullPath, _factory, _cache, entity);
                    task.Execute();
//                    CommonTaskQueue.Enqueue(new BackgroundCacheProvider(item.FullPath, _factory, _cache, entity));
                }

                if (entity.Duration > 0) { duration += entity.Duration; }
                if (entity.Children > 0) { grandchildren += entity.Children; }
            }

            if (_entity.Children == 0) { _entity.Children = count; };
            if (_entity.Duration == 0) { _entity.Duration = duration; }
            if ((grandchildren > 0) && (_entity.Kind.Equals(EntityKind.Artist))) { ((Artist)_entity).GrandChildren = grandchildren; }

            _entity.CalculateValues();
        }

        #endregion
    }
}
