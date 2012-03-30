using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Item : baseEntity
    {
        [DataMember]
        public int Progress { get; set; }

        public override string DefaultView
        {
            get { return "List"; }
        }

        public override string DefaultSort
        {
            get { return "[Title:sort]"; }
        }

        public override bool Playable
        {
            get { return true; }
        }
    }
}
