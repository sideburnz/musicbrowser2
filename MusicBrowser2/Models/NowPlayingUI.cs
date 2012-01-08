using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Models
{
    public class NowPlayingUI : BaseModel
    {
        private Foobar2000 _model = null;
        private bool _enabled = false;

        public NowPlayingUI()
        {
            ITransportEngine transport = TransportEngineFactory.GetEngine();
            _enabled = (Util.Helper.InheritsFrom<Foobar2000Transport>(transport));
            if (_enabled)
            {
                _model = Foobar2000.GetInstance();
                _model.PropertyChanged += new PropertyChangedEventHandler(_model_PropertyChanged);

                _model_PropertyChanged(null, "CurrentTrack");
                _model_PropertyChanged(null, "IsPlaying");
                _model_PropertyChanged(null, "IsPaused");
            }
        }

        void _model_PropertyChanged(IPropertyObject sender, string property)
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

        private bool _active = false;
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
