using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Item : baseEntity
    {
        private PlayState _playState;

        [DataMember]
        public int Progress { get; set; }

        protected override string DefaultView
        {
            get { return "List"; }
        }

        protected override string DefaultSort
        {
            get { return "[Title:sort]"; }
        }

        public override bool Playable
        {
            get { return true; }
        }

        public override IPlayState PlayState
        {
            get { return _playState ?? (_playState = new PlayState(CacheKey)); }
        }
    }
}
