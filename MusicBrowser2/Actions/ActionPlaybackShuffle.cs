using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    public class ActionPlaybackShuffle : baseActionCommand
    {
        private const string LABEL = "Shuffle Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconShuffle";

        private Foobar2000 _model;

        public ActionPlaybackShuffle(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlaybackShuffle()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlaybackShuffle(entity);
        }

        public Foobar2000 Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                _model.OnPropertyChanged += Listener;
                Listener("PlaybackStyle");
            }
        }

        private void Listener(string prop)
        {
            if (prop == "PlaybackStyle")
            {
                Alpha = Value ? 1 : 0.2F;
                FirePropertyChanged("Value");
            }
        }

        public bool Value
        {
            get
            {
                return (_model.PlaybackStyle == PlaybackStyles.Shuffle);
            }
            set { _model.SetPlaybackStyle(value ? PlaybackStyles.Shuffle : PlaybackStyles.Default); }
        }

        public override void DoAction(baseEntity entity)
        {
            Value = !Value;
        }
    }
}
