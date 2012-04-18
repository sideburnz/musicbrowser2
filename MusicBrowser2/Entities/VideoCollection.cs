using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Providers;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    [DataContract]
    class VideoCollection : Collection
    {
        public override string Information
        {
            get
            {
                return CalculateInformation("Video Collection", "Movie", "Show");
            }
        }

        public override void Play(bool queue, bool shuffle)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            MediaCollection collection = new MediaCollection();

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

            foreach (string item in playlist)
            {
                collection.AddItem(item);
                collection[collection.Count - 1].FriendlyData.Add("Title",
                                                                  System.IO.Path.GetFileNameWithoutExtension(item));
            }

            // if something is playing, stop it
            TransportEngineFactory.GetEngine().Stop();

            mce.PlayMedia(MediaType.MediaCollection, collection, false);
            mce.MediaExperience.GoToFullScreen();
        }
    }
}
