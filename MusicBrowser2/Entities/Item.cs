using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    abstract class Item : baseEntity
    {
        //public Item(string path) : base(path)

        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }
    }
}
