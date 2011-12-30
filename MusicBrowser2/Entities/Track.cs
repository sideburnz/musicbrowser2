using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Models;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Track : Music
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageTrack"; }
        }

        [DataMember]
        public string Artist { get; set; }

        [DataMember]
        public string Album { get; set; }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
