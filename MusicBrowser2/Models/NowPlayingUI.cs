﻿using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class NowPlayingUI : BaseModel
    {
        private readonly Foobar2000 _model;
        private readonly bool _enabled;

        public NowPlayingUI()
        {
            ITransportEngine transport = TransportEngineFactory.GetEngine();
            _enabled = (transport.InheritsFrom<Foobar2000Transport>());
            if (!_enabled) return;
            _model = Foobar2000.GetInstance();
            _model.PropertyChanged += ModelPropertyChanged;

            ModelPropertyChanged(null, "CurrentTrack");
            ModelPropertyChanged(null, "IsPlaying");
            ModelPropertyChanged(null, "IsPaused");
        }

        void ModelPropertyChanged(IPropertyObject sender, string property)
        {
            switch (property)
            {
                case "CurrentTrack":
                    if (_model.CurrentTrack.Path == "placeholder")
                    {
                        Active = false;
                    }
                    else
                    {
                        IconPath = _model.CurrentTrack.ThumbPath;
                        Active = true;
                    }
                    break;
                case "IsPlaying":
                case "IsPaused":
                    Active = (_model.IsPaused || _model.IsPlaying);
                    break;
            }
        }

        private bool _active;
        public bool Active
        {
            get
            {
                return _active && _enabled;
            }
            set
            {
                if (value != _active)
                {
                    _active = value;
                    FirePropertyChanged("Active");
                }
            }
        }

        private string _iconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageTrack";
        public string IconPath
        {
            get
            {
                return _iconPath;
            }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
                    FirePropertyChanged("IconPath");
                    FirePropertyChanged("Icon");
                }
            }
        }

        public Image Icon
        {
            get
            {
                return Util.Helper.GetImage(_iconPath);
            }
        }
    }
}
