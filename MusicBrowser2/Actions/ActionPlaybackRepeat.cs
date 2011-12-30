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
                if (Value)
                {
                    Alpha = 1;
                }
                else
                {
                    Alpha = 0.2F;
                }
                FirePropertyChanged("Value");
            }
        }

        public bool Value
        {
            get 
            {
                return (_model.PlaybackStyle == PlaybackStyles.RepeatPlaylist);
            }
            set
            {
                if (value)
                {
                    _model.SetPlaybackStyle(PlaybackStyles.RepeatPlaylist);
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
