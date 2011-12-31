using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Virtual : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override void Play(bool queue, bool shuffle)
        {
            throw new NotImplementedException();
        }
    }
}
