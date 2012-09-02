using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MusicBrowser.MediaCentre;
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
            List<string> playlist;

            if (Directory.Exists(Path))
            {
                // if we're dealing with a path, get all the tracks under the path
                playlist = FileSystemProvider.GetAllSubPaths(Path)
                    .FilterInternalFiles()
                    .Where(item => Helper.GetKnownType(item) == Helper.KnownType.Track)
                    .Select(item => item.FullPath)
                    .ToList();
            }
            else
            {
                // we're probably dealing with a .VF, so try to read the path info from it
                IFolderItemsProvider fip = new CollectionProvider();
                playlist = (from path in fip.GetItems(Path) from item in FileSystemProvider.GetAllSubPaths(path) where Helper.GetKnownType(item) == Helper.KnownType.Track select item.FullPath).ToList();
            }

            if (shuffle)
            {
                playlist = playlist.Shuffle().ToList();
            }

            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, playlist);

            // track play progress for restart
            ProgressRecorder.Start();
        }

    }
}
