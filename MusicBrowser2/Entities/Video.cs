using System;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.MediaCentre;

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
                    this.MarkPlayed();
                    mce.PlayMedia(MediaType.Dvd, Path, false);
                }
                else
                {
                    throw new NotImplementedException("Video.Play - when it's a folder and not a DVD");
                    //// refer it on to a more specialist Play action
                    //ActionPlayFolder a = new ActionPlayFolder(entity);
                    //a.Invoke();
                    //return;
                }
            }
            else
            {
                this.MarkPlayed();
                mce.PlayMedia(MediaType.Video, Path, queue);
            }
            mce.MediaExperience.GoToFullScreen();
            ProgressRecorder.Register(this);
        }
    }
}
