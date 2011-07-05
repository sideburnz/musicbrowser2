namespace MusicBrowser.Entities.Kinds
{
    class Playlist : IEntity
    {
        public Playlist()
        {
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imagePlaylist";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Playlist; }
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

        public override bool Playable
        {
            get
            {
                return true;
            }
        }

    }
}
