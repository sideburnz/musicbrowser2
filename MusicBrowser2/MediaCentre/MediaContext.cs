using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.MediaCentre
{
    /// <summary>
    /// This class makes consuming the built in MCE media transport etc cleaner and easier.
    /// It's expensive to monitor MCE like this so this is a singleton to prevent multiple instances.
    /// </summary>
    public class MediaContext
    {
        #region singleton
        static MediaContext _instance;
        static readonly object Padlock = new object();

        public static MediaContext GetInstance()
        {
            lock (Padlock)
            {
                if (_instance != null) return _instance;
                _instance = new MediaContext();
                return _instance;
            }
        }

        private MediaContext() 
        {
            _timer = new System.Timers.Timer { Interval = 1000 };
            _timer.Elapsed += delegate { SetUpMediaCenterContext(); };
            _timer.Enabled = true;
            _timer.Start();
        }
        #endregion

        #region exposed properties
        private IEntity _context;
        private PlayState _playState;
        private double _progress;
        private EntityFactory _factory;

        public IEntity Context
        {
            get { return _context; }
        }

        public PlayState PlayState
        {
            get { return _playState; }
        }

        public double Progress
        {
            get { return _progress; }
        }

        public EntityFactory Factory
        {
            set { _factory = value; }
        }

        #endregion

        #region events
        public delegate void EventHandler(object sender);
        public event EventHandler OnContextChanged;
        public event EventHandler OnProgressChanged;
        public event EventHandler OnPlayStateChanged;
        #endregion

        private readonly System.Timers.Timer _timer;
        private MediaExperience _mData = null;
        private double _lastProgress = 99.00;

        private void SetUpMediaCenterContext()
        {
            try
            {
                if (_mData == null)
                {
                    _mData = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.MediaExperience;
                    _mData.Transport.PropertyChanged += new PropertyChangedEventHandler(Transport_PropertyChanged);

                    RefreshMedia();

                    // once we're initialised, stop trying to initialise as often
                    _timer.Interval = 10000;
                }
            }
            catch { }
        }

        void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            if (property.ToLower().Equals("position"))
            {
                if (_context.Kind.Equals(EntityKind.Song))
                {
                    double trackPosition = _mData.Transport.Position.TotalSeconds;
                    long trackLength = long.Parse(_context.Properties["duration"]);
                    if (trackLength != 0)
                    {
                        _progress = trackPosition / trackLength;
                    }
                    else { _progress = 0; }
                }
                // if we're earlier in the track we're probably a different track (or the same track on repeat)
                if ((_lastProgress > _progress))
                {
                    RefreshMedia();
                    if (OnProgressChanged != null) OnProgressChanged(this);
                }
                // if we've played about 10% of the track
                if (_progress - _lastProgress >= 0.1)
                {
                    if (OnProgressChanged != null) OnProgressChanged(this);
                }
                _lastProgress = _progress;
                _playState = _mData.Transport.PlayState;
              
            }
            else if (property.ToLower().Equals("playstate"))
            {
                _playState = _mData.Transport.PlayState;
                if (OnPlayStateChanged != null) { OnPlayStateChanged(this); }
            }
        }

        private void RefreshMedia()
        {
            if (_mData.MediaMetadata.ContainsKey("TrackDuration"))
            {
                _context = _factory.getItem(_mData.MediaMetadata["Uri"].ToString());
                _playState = _mData.Transport.PlayState;
            }
            else
            {
                _context = new Unknown();
                _playState = PlayState.Undefined;
            }
            if (OnContextChanged != null) OnContextChanged(this);
            if (OnPlayStateChanged != null) { OnPlayStateChanged(this); }
        }
    }
}
