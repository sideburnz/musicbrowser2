using System;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Season : Folder
    {
        public override string DefaultSort
        {
            get { return "[Episode#:sort]"; }
        }

        public override string Information
        {
            get
            {
                string head = String.Empty;
                if (!String.IsNullOrEmpty(Show))
                {
                    head = Show + ",";
                }
                return CalculateInformation(head, "Episode");
            }
        }

        [DataMember]
        public string Show { get; set; }


        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
