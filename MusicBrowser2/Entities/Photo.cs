using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Models;
using ServiceStack.Text;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Photo : Item
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imagePhoto"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override void Play(bool queue, bool shuffle)
        {
            this.MarkPlayed();
            Application.GetReference().Navigate(this);
        }
    }
}
