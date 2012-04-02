using System;
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
    }
}
