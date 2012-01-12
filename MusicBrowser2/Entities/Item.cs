using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Models;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Item : baseEntity
    {
        [DataMember]
        public int Progress { get; set; }

        public override string DefaultView
        {
            get { return "List"; }
        }

        public override string DefaultSort
        {
            get { return "[Title:sort]"; }
        }

        public override string DefaultFormat
        {
            get { return "[Title]"; }
        }
    }
}
