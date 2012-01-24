using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Season : Folder
    {
        public override string DefaultSort
        {
            get { return "[Episode#:sort]"; }
        }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Episode");
            }
        }
    }
}
