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
    public abstract class Video : Item
    {
        public string Container
        {
            get { return _container; }
            set
            {
                if (value != _container)
                {
                    _container = value;
                    DataChanged("Container");
                }
            }
        }
        private string _container;

        public string AudioCodec
        {
            get { return _audiocodec; }
            set
            {
                if (value != _audiocodec)
                {
                    _audiocodec = value;
                    DataChanged("AudioCodec");
                }
            }
        }
        private string _audiocodec;

        public int AudioChannels { get; set; }

        public bool Subtitles { get; set; }

        public string VideoCodec { get; set; }
        public string AspectRatio { get; set; }


        public override string Information
        {
            get
            {
                return "container:" + Container + "," + TokenSubstitution("[duration]") + "," + Duration +
                    "  audio: " + AudioCodec + "," + AudioChannels +
                    "  video: " + VideoCodec + "," + AspectRatio +
                    "  subs: " + Subtitles.ToString();
            }
        }

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
            this.LastPlayed = DateTime.Now;
            ProgressRecorder.Register(this);
        }
    }
}
