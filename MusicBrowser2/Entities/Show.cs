using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Show : Folder
    {
        public override string Information
        {
            get
            {
                return CalculateInformation("", "Season", "Episode");
            }
        }
    }
}
