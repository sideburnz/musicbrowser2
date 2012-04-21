using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;

//TODO: test to make sure we have a context before trying to control it

namespace MusicBrowser.Engines.Transport
{
    class MediaCentreTransport : ITransportEngine
    {
        #region ITransport Members

        public void PlayPause()
        {
            MediaExperience mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience;
            if (mce.Transport.PlayState == Microsoft.MediaCenter.PlayState.Playing)
            {
                mce.Transport.PlayRate = 2;
            }
            else
            {
                mce.Transport.PlayRate = 1;
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
            List<string> tracks = files.ToList();

            // handle the first track
            if (mce.MediaExperience == null && queue) queue = false;
            mce.PlayMedia(MediaType.Audio, tracks.First(), queue);
            tracks.RemoveAt(0);
            // enqueue the rest of the tracks
            foreach (var track in tracks)
            {
                mce.PlayMedia(MediaType.Audio, track, true);   
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

        public bool ShowNowPlaying()
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce.MediaExperience != null)
            {
                mce.MediaExperience.GoToFullScreen();
                return true;
            }
            return false;
        }

        public bool HasBespokeNowPlaying
        {
            get { return false; }
        }

        #endregion

        private IEnumerable<string> GetTracks(string path)
        {
            List<string> ret = new List<string>();
            if (System.IO.Directory.Exists(path))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(path);
                ret.AddRange(from item in items where Util.Helper.GetKnownType(item) == Util.Helper.KnownType.Track select item.FullPath);
            }
            else
            {
                ret.Add(path);
            }
            return ret;
        }


        public void JumpForward()
        {
            throw new NotImplementedException();
        }

        public void JumpBack()
        {
            throw new NotImplementedException();
        }


        public bool IsPlaying
        {
            get
            {
                MediaExperience mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience;
                return (mce.Transport.PlayState == Microsoft.MediaCenter.PlayState.Playing);
            }
        }
    }
}
