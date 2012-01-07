using System;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.MediaCentre;
using MusicBrowser.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MusicBrowser.Entities
{
    [DataContract]
    abstract class Video : Item
    {
        [DataMember]
        public int Progress { get; set; }

        public override void Play(bool queue, bool shuffle)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (System.IO.Directory.Exists(Path))
            {
                if (Util.Helper.IsDVD(Path))
                {
                    mce.PlayMedia(MediaType.Dvd, Path, false);
                }
                else
                {
                    MediaCollection collection = new MediaCollection();
                    string lastitem = string.Empty;

                    List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path).
                        OrderBy(item => item.Name).
                        ToList();

                    foreach (FileSystemItem item in candidateitems)
                    {
                        var t = Util.Helper.getKnownType(item);
                        if (t == Util.Helper.knownType.Video)
                        {
                            collection.AddItem(item.FullPath);
                            collection[collection.Count - 1].FriendlyData.
                                Add("Title", 
                                Title + " (" + System.IO.Path.GetFileNameWithoutExtension(item.Name) + ")");
                            lastitem = item.FullPath;
                        }
                    }

                    // if there's only 1 item, just play it
                    if (collection.Count == 1)
                    {
                        mce.PlayMedia(MediaType.Video, lastitem, false);
                    }
                    else
                    {
                        mce.PlayMedia(MediaType.MediaCollection, collection, false);
                    }
                }
            }
            else
            {
                mce.PlayMedia(MediaType.Video, Path, false);
            }

            mce.MediaExperience.GoToFullScreen();
            ProgressRecorder.Register(this);
        }
    }
}
