using System;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.MediaCentre;
using MusicBrowser.Providers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MusicBrowser.Util;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Video : Item
    {
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public int AudioChannels
        {
            get { return _audiochannels; }
            set
            {
                if (value != _audiochannels)
                {
                    _audiochannels = value;
                    DataChanged("AudioChannels");
                }
            }
        }
        private int _audiochannels;
        [DataMember]
        public bool Subtitles
        {
            get { return _subtitles; }
            set
            {
                if (value != _subtitles)
                {
                    _subtitles = value;
                    DataChanged("Subtitles");
                }
            }
        }
        private bool _subtitles;
        [DataMember]
        public string VideoCodec
        {
            get { return _videocodec; }
            set
            {
                if (value != _videocodec)
                {
                    _videocodec = value;
                    DataChanged("VideoCodec");
                }
            }
        }
        private string _videocodec;
        [DataMember]
        public string AspectRatio
        {
            get { return _aspectratio; }
            set
            {
                if (value != _aspectratio)
                {
                    _aspectratio = value;
                    DataChanged("AspectRatio");
                }
            }
        }
        private string _aspectratio;
        [DataMember]
        public bool HiDef
        {
            get { return _hd; }
            set
            {
                if (value != _hd)
                {
                    _hd = value;
                    DataChanged("HiDef");
                }
            }
        }
        private bool _hd;

        public override string Information
        {
            get
            {
                return "(" + TokenSubstitution("[duration]") + ")";
            }
        }

        public override List<Image> CodecIcons
        {
            get
            {
                List<Image> ret = new List<Image>();

                try
                {

                    if (Util.Helper.IsDVD(Path))
                    {
                        ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/container_dvd"));
                    }
                    else
                    {
                        ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/container_" + Container.ToLower()));
                    }

                    ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/codec_" + VideoCodec.ToLower()));
                    ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/codec_" + AudioCodec.ToLower()));

                    if (HiDef)
                    {
                        ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/HD"));
                    }
                    if (Subtitles)
                    {
                        ret.Add(Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/Subs"));
                    }
                }
                catch { }
                return ret;
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

                    List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(Path)
                        .FilterDVDFiles()
                        .OrderBy(item => item.Name)
                        .ToList();

                    if (shuffle) { Util.Helper.ShuffleList<FileSystemItem>(candidateitems); }

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
