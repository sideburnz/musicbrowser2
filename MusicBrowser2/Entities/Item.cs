using System.Runtime.Serialization;
using MusicBrowser.Engines.PlayState;
using MusicBrowser.Engines.ViewState;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Item : baseEntity
    {
        private PlayState _playState;

        public override IViewState ViewState
        {
            get { return new ItemViewState("item"); }
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
