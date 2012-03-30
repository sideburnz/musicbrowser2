using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Util;

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
            IFolderItemsProvider fip = new CollectionProvider();
            List<string> playlist = (from path in fip.GetItems(Path) from item in FileSystemProvider.GetAllSubPaths(path) where Helper.GetKnownType(item) == Helper.KnownType.Track select item.FullPath).ToList();

            if (shuffle)
            {
                playlist = playlist.Shuffle().ToList();
            }

            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, playlist);
        }
    }
}
