using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Entities
{
    interface IPlaylistable
    {
        void Play();
        void Queue();
        void Shuffle();
    }
}
