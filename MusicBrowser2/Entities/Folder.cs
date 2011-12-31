using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.Text;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;
using System.IO;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Folder : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override void Play(bool queue, bool shuffle)
        {
            // determine if it has move images, music or videos
            // photos can't be queued at the moment, so say so
            // music should be added to a virtual playlist and passed to the transport
            // videos should be queued up and played

            throw new NotImplementedException();

            //MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;

            //List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path).
            //    OrderBy(item => item.Name).
            //    ToList();

            //if (shuffle)
            //{
            //    Util.Helper.ShuffleList<FileSystemItem>(candidateitems);
            //}

            //foreach (FileSystemItem item in candidateitems)
            //{
            //    var t = Util.Helper.getKnownType(item);
            //    if (t == Util.Helper.knownType.Video || t == Util.Helper.knownType.Track)
            //    {
            //        collection.AddItem(item.FullPath);
            //        collection[collection.Count - 1].FriendlyData.Add("Title", System.IO.Path.GetFileNameWithoutExtension(item.Name));
            //        lastitem = item.FullPath;
            //    }
            //}

            //Models.UINotifier.GetInstance().Message = String.Format("playing {0} folder items", collection.Count());
            //this.MarkPlayed();
            //mce.PlayMedia(MediaType.MediaCollection, collection, queue);
            //mce.MediaExperience.GoToFullScreen();
        }
    }
}
