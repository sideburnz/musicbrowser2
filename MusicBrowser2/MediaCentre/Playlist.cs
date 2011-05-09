using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using MusicBrowser.Providers.Transport;

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
                        Transport.getTransport().PlayPause();
                        return;
                    }
                case "cmdpause":
                    {
                        Transport.getTransport().PlayPause();
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

            Transport.getTransport().Play(queue, tracks);

            AutoShowNowPlaying();
        }

        public static void PlaySong(IEntity entity, Boolean add)
        {
            Transport.getTransport().Play(add, entity.Path);
            AutoShowNowPlaying();
        }

        public static void PlayDisc(IEntity entity)
        {
            Transport.getTransport().PlayDisc(entity.Path);
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