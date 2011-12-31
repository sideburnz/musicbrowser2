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
            // music folders 
            //switch (entity.Kind)
            //{
            //    case EntityKind.Album:
            //    case EntityKind.Artist:
            //    case EntityKind.Genre:
            //        TransportEngineFactory.GetEngine().Play(false, entity.Path);
            //        return;
            //    case EntityKind.PhotoAlbum:
            //        Models.UINotifier.GetInstance().Message = String.Format("playing image folders doesn't work");
            //        return;
            //}

            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            MediaCollection collection = new MediaCollection();
            string lastitem = string.Empty;

            // it's a playlist of files so
            // get them, weed out the non-video, sort them and play them
            List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path).
                OrderBy(item => item.Name).
                ToList();

            foreach (FileSystemItem item in candidateitems)
            {
                var t = Util.Helper.getKnownType(item);
                if (t == Util.Helper.knownType.Video) // || t == Util.Helper.knownType.Track || t == Util.Helper.knownType.Image)
                {
                    collection.AddItem(item.FullPath);
                    collection[collection.Count - 1].FriendlyData.Add("Title", Title + " (" + System.IO.Path.GetFileNameWithoutExtension(item.Name) + ")");
                    lastitem = item.FullPath;
                }
            }

            // if there's only 1 item, just play it (this may go wrong if it's not a video)
            if (collection.Count == 1)
            {
                this.MarkPlayed();
                mce.PlayMedia(MediaType.Video, lastitem, queue);
                mce.MediaExperience.GoToFullScreen();
                return;
            }

            Models.UINotifier.GetInstance().Message = String.Format("playing {0} folder items", collection.Count());
            this.MarkPlayed();
            mce.PlayMedia(MediaType.MediaCollection, collection, queue);
            mce.MediaExperience.GoToFullScreen();
        }
    }
}
