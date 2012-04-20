using System.Runtime.Serialization;
using MusicBrowser.Engines.PlayState;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Item : baseEntity
    {
        private PlayState _playState;

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
