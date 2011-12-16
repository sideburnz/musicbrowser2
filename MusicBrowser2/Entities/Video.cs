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
    abstract class Video : Item
    {
        [DataMember]
        public int Progress { get; set; }
    }
}
