using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Engines.Actions
{
    public class ActionPlaybackRepeat : baseActionCommand
    {
        private const string LABEL = "Repeat Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRepeat";

        private Foobar2000 _model;

        public ActionPlaybackRepeat(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlaybackRepeat()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlaybackRepeat(entity);
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
                return (_model.PlaybackStyle == PlaybackStyles.RepeatPlaylist);
            }
            set { _model.SetPlaybackStyle(value ? PlaybackStyles.RepeatPlaylist : PlaybackStyles.Default); }
        }

        public override void DoAction(baseEntity entity)
        {
            Value = !Value;
        }
    }
}
