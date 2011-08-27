using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Transport;
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
                        Transport.GetTransport().PlayPause();
                        return;
                    }
                case "cmdpause":
                    {
                        Transport.GetTransport().PlayPause();
                        return;
                    }
            }

            //this needs to be put on a background thread otherwise it freezes the app
            PlaylistProvider PP = new PlaylistProvider(Action, Entity);
            Providers.Background.CommonTaskQueue.Enqueue(PP, true);
        }
            
        public static void PlayTrackList(IEnumerable<string> tracks, bool queue)
        {
            Models.UINotifier.GetInstance().Message = string.Format("Adding {0} tracks to playlist", tracks.Count());
            Transport.GetTransport().Play(queue, tracks);
            AutoShowNowPlaying();
        }

        public static void PlaySong(IEntity entity, Boolean add)
        {
            Transport.GetTransport().Play(add, entity.Path);
            AutoShowNowPlaying();
        }

        public static void PlayDisc(IEntity entity)
        {
            Transport.GetTransport().PlayDisc(entity.Path);
            AutoShowNowPlaying();
        }

        private static void AutoShowNowPlaying()
        {
            if (Config.GetInstance().GetBooleanSetting("AutoLoadNowPlaying"))
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