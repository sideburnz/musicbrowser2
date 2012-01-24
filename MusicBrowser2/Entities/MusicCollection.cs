using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;

namespace MusicBrowser.Entities
{
    [DataContract]
    class MusicCollection : Collection
    {
        public override string Information
        {
            get
            {
                return CalculateInformation("Music Collection", "Artist", "Album");
            }
        }

        public override void Play(bool queue, bool shuffle)
        {
            List<string> playlist = new List<string>();

            IFolderItemsProvider fip = new CollectionProvider();
            foreach (string path in fip.GetItems(Path))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(path);
                foreach (FileSystemItem item in items)
                {
                    if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Track)
                    {
                        playlist.Add(item.FullPath);
                    }
                }
            }

            if (shuffle)
            {
                Util.Helper.ShuffleList<string>(playlist);
            }

            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, playlist);
        }
    }
}
