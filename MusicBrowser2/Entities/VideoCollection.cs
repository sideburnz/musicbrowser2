using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    [DataContract]
    class VideoCollection : Collection
    {
        public override void Play(bool queue, bool shuffle)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            MediaCollection collection = new MediaCollection();

            List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path).
                OrderBy(item => item.Name).
                ToList();

            foreach (FileSystemItem item in candidateitems)
            {
                if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Video)
                {
                    collection.AddItem(item.FullPath);
                    collection[collection.Count - 1].FriendlyData.Add("Title", System.IO.Path.GetFileNameWithoutExtension(item.Name));
                }
            }

            if (shuffle)
            {
                Util.Helper.ShuffleList<FileSystemItem>(candidateitems);
            }

            mce.PlayMedia(MediaType.MediaCollection, collection, false);
            mce.MediaExperience.GoToFullScreen();
        }
    }
}
