using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    public class ActionPlaybackShuffle : baseActionCommand
    {
        private const string LABEL = "Shuffle Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/checkUnselected";

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
                if (Value)
                {
                    IconPath = "resx://MusicBrowser/MusicBrowser.Resources/checkSelected";
                }
                else
                {
                    IconPath = "resx://MusicBrowser/MusicBrowser.Resources/checkUnselected";
                }
                FirePropertyChanged("Value");

                Engines.Logging.LoggerEngineFactory.Debug("PlaybackStyle: " + IconPath);
            }
        }

        public bool Value
        {
            get
            {
                return (_model.PlaybackStyle == PlaybackStyles.Shuffle);
            }
            set
            {
                if (value)
                {
                    _model.SetPlaybackStyle(PlaybackStyles.Shuffle);
                }
                else
                {
                    _model.SetPlaybackStyle(PlaybackStyles.Default);
                }
            }
        }

        public override void DoAction(baseEntity entity)
        {
            Value = !Value;
        }
    }
}
