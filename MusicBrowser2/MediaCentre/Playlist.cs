using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Util;

namespace MusicBrowser.MediaCentre
{
    public class Playlist
    {
        public static void DoAction(string Action, Entity Entity)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Playlist.DoAction(" + Action + ", " + Entity.Kind.ToString() + ": " + Entity.Path + ")", "start");
#endif
            if (Entity == null) { return; }

            switch (Action.ToLower())
            {
                // handle these in real-time
                case "cmdresume":
                    {
                        TransportEngineFactory.GetEngine().PlayPause();
                        return;
                    }
                case "cmdpause":
                    {
                        TransportEngineFactory.GetEngine().PlayPause();
                        return;
                    }
            }

            //this needs to be put on a background thread otherwise it freezes the app
            PlaylistProvider PP = new PlaylistProvider(Action, Entity);
            Providers.Background.CommonTaskQueue.Enqueue(PP, true);
        }
            
        public static void PlayTrackList(IEnumerable<string> tracks, bool queue)
        {
            TransportEngineFactory.GetEngine().Play(queue, tracks);
            AutoShowNowPlaying();
        }

        public static void PlayTrack(Entity entity, Boolean add)
        {
            TransportEngineFactory.GetEngine().Play(add, entity.Path);
            AutoShowNowPlaying();
        }

        public static void PlayDisc(Entity entity)
        {
            TransportEngineFactory.GetEngine().PlayDisc(entity.Path);
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