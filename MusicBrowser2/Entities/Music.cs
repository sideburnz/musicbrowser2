using System;
using System.Runtime.Serialization;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Music : Item
    {

        public override void Play(bool queue, bool shuffle)
        {
            if (queue)
            {
                Models.UINotifier.GetInstance().Message = String.Format("queuing {0}", Title);
            }
            else
            {
                Models.UINotifier.GetInstance().Message = String.Format("playing {0}", Title);
            }

            TransportEngineFactory.GetEngine().Play(queue, Path);
            MediaCentre.Playlist.AutoShowNowPlaying();
            
            //these aren't critical, so wait to do these
            PlayState.Play();
            this.UpdateCache();
        }
    }
}
