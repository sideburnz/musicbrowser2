using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MusicBrowser.Engines.PlayState;
using MusicBrowser.Engines.ViewState;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Container : baseEntity
    {
        private IViewState _viewState;

        public override IViewState ViewState
        {
            get
            {
                return _viewState ?? (_viewState = new ContainerViewState(CacheKey)
                {
                    DefaultSort = "[Title:sort]",
                    DefaultView = "List"
                });
            }
        }

        public override bool Playable
        {
            get { return false; }
        }

        public override IPlayState PlayState
        {
            get { return new BlankPlayState(); }
        }
    }
}
