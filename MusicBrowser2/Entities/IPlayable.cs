using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Entities
{
    interface IPlayable
    {
        void Play();
        void Queue();
    }
}
