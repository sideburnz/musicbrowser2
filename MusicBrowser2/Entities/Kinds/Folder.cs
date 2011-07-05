namespace MusicBrowser.Entities.Kinds
{
    class Folder : IEntity
    {
        public Folder()
        {
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageFolder";
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
