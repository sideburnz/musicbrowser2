using System.Runtime.Serialization;

namespace MusicBrowser.Entities.Kinds
{
    [DataContract]
    [KnownType(typeof(Unknown))]
    class Unknown : IEntity
    {
        public Unknown()
        {
            Title = "[unknown]";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Unknown; }
        }
    }
}
