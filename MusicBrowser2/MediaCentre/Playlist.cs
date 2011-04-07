using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.MediaCentre
{
    public class Playlist
    {
        public static void DoAction(string Action, IEntity Entity)
        {
#if DEBUG
            Logging.Logger.Verbose("Playlist.DoAction(" + Action + ", " + Entity.Kind.ToString() + ": " + Entity.Path + ")", "start");
#endif
            if (Entity.Kind.Equals(EntityKind.Unknown)) { return; }

            switch (Action.ToLower())
            {
                // handle these in real-time
                case "cmdresume":
                    {
                        Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 2;
                        return;
                    }
                case "cmdpause":
                    {
                        Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PlayRate = 1;
                        return;
                    }
            }

            //TODO: this should be put on a background thread otherwise it freezes the app
            PlaylistProvider PP = new PlaylistProvider(Action, Entity);
            PP.Execute();
        }
            
        public static void PlayTrackList(IEnumerable<string> tracks, bool queue)
        {
            Models.UINotifier.GetInstance().Message = "Adding " + tracks.Count() + " tracks to playlist";

            // set up
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            // handle the first track
            if (mce.MediaExperience == null && queue == true) queue = false;
            mce.PlayMedia(MediaType.Audio, tracks.First(), queue);
            // enqueue the rest of the tracks
            for (int i = 1; i < tracks.Count(); i++)
            {
                mce.PlayMedia(MediaType.Audio, tracks.ElementAt(i), true);
            }

            AutoShowNowPlaying();
        }

        public static void PlaySong(IEntity entity, Boolean add)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce.MediaExperience == null && add == true) add = false;
            mce.PlayMedia(MediaType.Audio, entity.Path, add);

            AutoShowNowPlaying();
        }

        private static void AutoShowNowPlaying()
        {
            if (Config.getInstance().getBooleanSetting("AutoLoadNowPlaying"))
            {
                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                if (mce.MediaExperience != null)
                {
                    mce.MediaExperience.GoToFullScreen();
                }
            }
        }

    }
}