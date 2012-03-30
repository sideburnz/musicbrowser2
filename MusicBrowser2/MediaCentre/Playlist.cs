using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.MediaCentre
{
    public class Playlist
    {            
        public static void PlayTrackList(IEnumerable<string> tracks, bool queue)
        {
            TransportEngineFactory.GetEngine().Play(queue, tracks);
        }

        public static void PlayTrack(baseEntity entity, Boolean add)
        {
            TransportEngineFactory.GetEngine().Play(add, entity.Path);
        }

        public static void AutoShowNowPlaying()
        {
            if (Config.GetInstance().GetBooleanSetting("AutoLoadNowPlaying"))
            {
                ITransportEngine t = TransportEngineFactory.GetEngine();
                if (t.HasBespokeNowPlaying)
                {
                    if (TransportEngineFactory.GetEngine().ShowNowPlaying())
                    {
                        return;
                    }
                }
                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                if (mce.MediaExperience != null)
                {
                    mce.MediaExperience.GoToFullScreen();
                }
            }
        }

    }
}