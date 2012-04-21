using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.Text;

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

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override List<string> SortFields
        {
            get
            {
                return new List<string>
                           {
                               "Season#",
                               "Title",
                               "Filename",
                               "Added"
                           };
            }
        }
    }
}
