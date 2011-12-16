using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Folder : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
