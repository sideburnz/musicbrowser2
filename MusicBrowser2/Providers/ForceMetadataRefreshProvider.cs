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
        private Entity _parent;

        public ForceMetadataRefreshProvider(Entity parent)
        {
            _parent = parent;
        }

        public string Title
        {
            get { return "ForceMetadataRefreshProvider(" + _parent.Path +")"; }
        }

        public void Execute()
        {
            // refresh the children item
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(_parent.Path);
            foreach (FileSystemItem item in items)
            {
                Entity e = EntityFactory.GetItem(item, true);
                new Metadata.MetadataProviderList(e, true).Execute();
            }

            // refresh the current item last
            _parent = EntityFactory.GetItem(_parent.Path, true);
            new Metadata.MetadataProviderList(_parent, true).Execute();
        }
    }
}
