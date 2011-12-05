using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Virtual : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }

        public override string Kind
        {
            get { return "Virtual"; }
        }
    }
}
