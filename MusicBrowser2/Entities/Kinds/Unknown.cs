namespace MusicBrowser.Entities.Kinds
{
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
