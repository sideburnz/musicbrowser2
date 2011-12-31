using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Models;
using MusicBrowser.Engines.Cache;
using MusicBrowser.MediaCentre;
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

            this.MarkPlayed();
            TransportEngineFactory.GetEngine().Play(queue, Path);
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
