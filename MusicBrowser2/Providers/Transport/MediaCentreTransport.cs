using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter;

namespace MusicBrowser.Providers.Transport
{
    class MediaCentreTransport : ITransport
    {
        #region ITransport Members

        public void PlayPause()
        {
        }

        public void Play(bool queue, string file)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce.MediaExperience == null && queue) queue = false;
            mce.PlayMedia(MediaType.Audio, file, queue);
        }

        public void Play(bool queue, IEnumerable<string> files)
        {
            // set up
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            // handle the first track
            if (mce.MediaExperience == null && queue) queue = false;
            mce.PlayMedia(MediaType.Audio, files.First(), queue);
            // enqueue the rest of the tracks
            for (int i = 1; i < files.Count(); i++)
            {
                mce.PlayMedia(MediaType.Audio, files.ElementAt(i), true);
            }
        }

        public void PlayDisc(string drive)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            mce.PlayMedia(MediaType.Audio, drive, false);
        }

        public void Stop()
        {
 //           Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 0;
        }

        public void Next()
        {
//            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.SkipForward();
        }

        public void Previous()
        {
//            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.SkipBack();
        }

        public void Close()
        {
            // do nothing, for external players only
        }

        public int Progress
        {
            get
            {
                try
                {
                    return Int32.Parse(Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position.TotalSeconds.ToString());
                }
                catch { }
                return -1;
            }
        }

        public PlayState State
        {
            get
            {
                try
                {
                    switch (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayState)
                    {
                        case Microsoft.MediaCenter.PlayState.Paused:
                            {
                                return PlayState.Paused;
                            }
                        case Microsoft.MediaCenter.PlayState.Playing:
                            {
                                return PlayState.Playing;
                            }
                    }
                }
                catch { }
                return PlayState.Undefined;
            }
        }

        #endregion
    }
}
