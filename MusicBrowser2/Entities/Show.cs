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
        [DataMember]
        String SeriesID { get; set; }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Season", "Episode");
            }
        }
    }
}
