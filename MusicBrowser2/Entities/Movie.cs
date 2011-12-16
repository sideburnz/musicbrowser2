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
    class Movie : Video
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageVideo"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
