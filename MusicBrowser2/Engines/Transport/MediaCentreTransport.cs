using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers;

namespace MusicBrowser.Engines.Transport
{
    class MediaCentreTransport : ITransportEngine
    {
        #region ITransport Members

        public void PlayPause()
        {
            if (Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayState == PlayState.Playing)
            {
                Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 1;
            }
            else
            {
                Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 2;
            }
        }

        // unlike other transport providers, Media Center needs to be passed individual files
        // rather than the folder that contains them. So we get all of the contents of a folder
        // and use the overloaded play method for lists. This also works for lists of one item
        // if we've been passed a single track.
        public void Play(bool queue, string file)
        {
            IEnumerable<string> paths = GetTracks(file);
            Play(queue, paths);
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

        public void Stop()
        {
            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 0;
        }

        public void Next()
        {
            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.SkipForward();
        }

        public void Previous()
        {
            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.SkipBack();
        }

        public void Close()
        {
            // do nothing, for external players only
        }

        public void Open()
        {
            // do nothing
        }

        #endregion

        private IEnumerable<string> GetTracks(string path)
        {
            List<string> ret = new List<string>();
            if (System.IO.Directory.Exists(path))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(path);
                foreach (FileSystemItem item in items)
                {
                    if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Track)
                    {
                        ret.Add(item.FullPath);
                    }
                }
            }
            else
            {
                ret.Add(path);
            }
            return ret;
        }
    }
}
