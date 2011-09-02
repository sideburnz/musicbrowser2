using System.Runtime.Serialization;

namespace MusicBrowser.Entities.Kinds
{
    [DataContract]
    [KnownType(typeof(Folder))]
    class Folder : IEntity
    {
        public override string DefaultIconPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Folder; }
        }

        public override string ShortSummaryLine1
        {
            get
            {
                if (string.IsNullOrEmpty(base.ShortSummaryLine1))
                {
                    return Kind.ToString();
                }
                return base.ShortSummaryLine1;
            }
            set { base.ShortSummaryLine1 = value; }
        }
    }
}
