using System;
using System.Xml;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.Models
{
    // now playing page for Foobar2000 transport

    public enum PlaybackStyles
    {
        Shuffle,
        RepeatTrack,
        RepeatPlaylist,
        Default
    }

    public enum FoobarInternalPlaybackStyles
    {
        Default = 0,
        RepeatPlaylist = 1,
        RepeatTrack = 2,
        Random = 3,
        ShuffleTracks = 4,
        ShuffleAlbums = 5,
        ShuffleFolders = 6
    }

    public struct PlayerInformation
    {
        public bool IsPlaying;
        public bool IsPaused;
        public PlaybackStyles PlaybackStyle;

        public int PlaylistSize;
        public int PlaylistCurrentPage;
        public int PlaylistPageSize;
        public string PlaylistDuration;
        public int PlaylistItemPlaying;

        public string PlayingTrackArtist;
        public string PlayingTrackName;
        public string PlayingTrackNumber;
        public string PlayingTrackAlbumArtist;
        public string PlayingTrackAlbumName;
        public string PlayingTrackPath;

        public int PlayingTrackProgress;
        public int PlayingTrackLength;
    }

    public class Foobar2000 : Foobar2000Transport
    {
        private readonly Timer _timer;
        
        private static readonly Foobar2000 Instance = new Foobar2000();

        public static Foobar2000 GetInstance()
        {
            return Instance;
        }

        private Foobar2000()
        {
            _timer = new Timer(this)
            {
                Interval = 1000,
                AutoRepeat = true,
                Enabled = (Config.GetInstance().GetStringSetting("Player.Engine").ToLower() == "foobar2000")
            };
            _timer.Tick += delegate { OnTick(); };
        }

        private PlaybackStyles _playbackstyle = PlaybackStyles.Default;
        public PlaybackStyles PlaybackStyle
        {
            get
            {
                return _playbackstyle;
            }
            set
            {
                if (value != _playbackstyle)
                {
                    _playbackstyle = value;
                    DataChanged("PlaybackStyle");
                }
            }
        }

        private int _playlistpage;
        public int PlaylistPage 
        {
            get
            {
                return _playlistpage;
            }
            protected set
            {
                if (value != _playlistpage)
                {
                    _playlistpage = value;
                    DataChanged("PlaylistPage");
                }
            }
        }
        private int _playlistPages;
        public int PlaylistPages 
        {
            get
            {
                return _playlistPages;
            }
            protected set
            {
                if (value != _playlistPages)
                {
                    _playlistPages = value;
                    DataChanged("PlaylistPages");
                }
            }
        }
        private int _playlistLength;
        public int PlaylistLength
        {
            get
            {
                return _playlistLength;
            }
            protected set
            {
                if (value != _playlistLength)
                {
                    _playlistLength = value;
                    DataChanged("PlaylistLength");
                }
            }
        }
        private int _playlistItem;
        public int PlaylistItem
        {
            get
            {
                return _playlistItem;
            }
            protected set
            {
                if (value != _playlistItem)
                {
                    _playlistItem = value;
                    DataChanged("PlaylistItem");
                }
            }
        }
        private string _playlistDuration = "0:00";
        public string PlaylistDuration
        {
            get
            {
                return _playlistDuration;
            }
            protected set
            {
                if (value != _playlistDuration)
                {
                    _playlistDuration = value;
                    DataChanged("PlaylistDuration");
                }
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    DataChanged("IsPlaying");
                }
            }
        }

        private bool _isPaused;
        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }
            set
            {
                if (_isPaused != value)
                {
                    _isPaused = value;
                    DataChanged("IsPaused");
                }
            }
        }

        // context information
        private Track _currentTrack;
        public Track CurrentTrack 
        {
            get
            {
                if (_currentTrack != null)
                {
                    return _currentTrack;
                }
                return new Track { Path = "placeholder" };
            }
            protected set
            {

                if (value == null)
                {
                    if (_currentTrack != null)
                    {
                        _currentTrack = null;
                        DataChanged("CurrentTrack");
                    }
                    return;
                }
                if (CurrentTrack.CacheKey != value.CacheKey)
                {
                    _currentTrack = value;
                    DataChanged("CurrentTrack");
                }
            }
        }

        private int _position;
        public int Position 
        { 
            get
            {
                return _position;
            }
            set
            {
                if (value == _position) return;
                _position = value;
                if (ReportedLength > 0)
                {
                    PercentComplete = (100.00*_position)/ReportedLength;
                }

                TimeSpan t = TimeSpan.FromSeconds(_position);
                if (t.Hours == 0)
                {
                    ProgressText = string.Format("{0}:{1:D2}", (Int32) Math.Floor(t.TotalMinutes), t.Seconds);
                }
                else
                {
                    ProgressText = string.Format("{0}:{1:D2}:{2:D2}", (Int32) Math.Floor(t.TotalHours), t.Minutes, t.Seconds);
                }

                DataChanged("Position");
                DataChanged("PercentComplete");
                DataChanged("ProgressBar");
                DataChanged("ProgressText");
            }
        }

        public double PercentComplete { get; set; }
        public string ProgressText { get; set; }

        private int _reportedLength;
        public int ReportedLength
        {
            get 
            { 
                return _reportedLength;
            }
            set
            {
                if (value == _reportedLength) return;
                _reportedLength = value;                    
                DataChanged("ReportedLength");
            }
        }

        public void SetPlaybackStyle(PlaybackStyles style)
        {
            int enumeration = 0;
            switch (style)
            {
                case PlaybackStyles.Default:
                    enumeration = 0; break;
                case PlaybackStyles.RepeatPlaylist:
                    enumeration = 1; break;
                case PlaybackStyles.RepeatTrack:
                    enumeration = 2; break;
                case PlaybackStyles.Shuffle:
                    enumeration = 3; break;
            }
            ExecuteCommand("PlaybackOrder", enumeration.ToString());
        }

        void Seek(int location)
        {
            ExecuteCommand("SeekDelta=" + (_position + location));
        }

        private PlayerInformation ProcessResponse(string xml)
        {
            PlayerInformation result = new PlayerInformation();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml);
                result.IsPaused = (Helper.ReadXmlNode(xmldoc, "/foobar2000/state/IS_PAUSED", "0") == "1");
                result.IsPlaying = (Helper.ReadXmlNode(xmldoc, "/foobar2000/state/IS_PLAYING", "0") == "1");

                int i;
                if (Int32.TryParse(Helper.ReadXmlNode(xmldoc, "/foobar2000/state/PLAYBACK_ORDER", "0"), out i))
                {
                    FoobarInternalPlaybackStyles fbs = (FoobarInternalPlaybackStyles)i;
                    switch (fbs)
                    {
                        case FoobarInternalPlaybackStyles.Default:
                            {
                                result.PlaybackStyle = PlaybackStyles.Default;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.Random:
                        case FoobarInternalPlaybackStyles.ShuffleAlbums:
                        case FoobarInternalPlaybackStyles.ShuffleFolders:
                        case FoobarInternalPlaybackStyles.ShuffleTracks:
                            {
                                result.PlaybackStyle = PlaybackStyles.Shuffle;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.RepeatPlaylist:
                            {
                                result.PlaybackStyle = PlaybackStyles.RepeatPlaylist;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.RepeatTrack:
                            {
                                result.PlaybackStyle = PlaybackStyles.RepeatTrack;
                                break;
                            }
                    }
                }
                else
                {
                    result.PlaybackStyle = PlaybackStyles.Default;
                }


                if (Int32.TryParse(Helper.ReadXmlNode(xmldoc, "/foobar2000/item/ITEM_PLAYING_POS", "0"), out i))
                {
                    result.PlayingTrackProgress = i;
                }
                else
                {
                    result.PlayingTrackProgress = 0;
                }

                if (Int32.TryParse(Helper.ReadXmlNode(xmldoc, "/foobar2000/item/ITEM_PLAYING_LEN", "0"), out i))
                {
                    result.PlayingTrackLength = i;
                }
                else
                {
                    result.PlayingTrackLength = 0;
                }

                result.PlayingTrackPath = Helper.ReadXmlNode(xmldoc, "/foobar2000/item/path", String.Empty);

                if (Int32.TryParse(Helper.ReadXmlNode(xmldoc, "/foobar2000/playlist/PLAYLIST_PLAYING_ITEMS_COUNT", "0"), out i))
                {
                    result.PlaylistSize = i;
                }
                else
                {
                    result.PlaylistSize = 0;
                }

                if (Int32.TryParse(Helper.ReadXmlNode(xmldoc, "/foobar2000/playlist/PLAYLIST_ITEM_PLAYING", "0"), out i))
                {
                    result.PlaylistItemPlaying = i;
                }
                else
                {
                    result.PlaylistItemPlaying = 0;
                }

                result.PlaylistDuration = Helper.ReadXmlNode(xmldoc, "/foobar2000/playlist/PLAYLIST_TOTAL_TIME", "0:00");

            }
            catch { }

            return result;
        }

        private static string _path = String.Empty;
        public void OnTick()
        {
            string xml = ExecuteCommand("RefreshPlayingInfo");
            PlayerInformation info;

            if (!String.IsNullOrEmpty(xml))
            {
                info = ProcessResponse(xml);
            }
            else
            {
                info = new PlayerInformation();
            }

            PlaybackStyle = info.PlaybackStyle;

            PlaylistPage = info.PlaylistCurrentPage;
            PlaylistPages = (int)Math.Ceiling(info.PlaylistSize / (double)info.PlaylistPageSize);

            Position = info.PlayingTrackProgress;

            if (_path != info.PlayingTrackPath)
            {
                CurrentTrack = (Track)EntityFactory.GetItem(info.PlayingTrackPath);
                _path = info.PlayingTrackPath;
            }

            IsPaused = info.IsPaused;
            IsPlaying = info.IsPlaying;

            PlaylistDuration = info.PlaylistDuration;
            PlaylistLength = info.PlaylistSize;
            PlaylistItem = info.PlaylistItemPlaying + 1;

            ReportedLength = info.PlayingTrackLength;
        }

        private void DataChanged(string property)
        {
            FirePropertyChanged(property);
            if(OnPropertyChanged != null) 
            {
                OnPropertyChanged(property); 
            }
        }

        public delegate void ChangedPropertyHandler(string property);
        public event ChangedPropertyHandler OnPropertyChanged;

        public Size ProgressBar
        {
            get
            {
                Size s = new Size {Height = 30, Width = (int) (PercentComplete*4)};
                return s;
            }
        }

        public bool ScreenSaverDisabled
        {
            get
            {
                return Config.GetInstance().GetBooleanSetting("Player.DisableScreenSaver");
            }
        }
    }
}


//<foobar2000>
//    <state>
//        <IS_PLAYING>1</IS_PLAYING>
//        <IS_PAUSED>0</IS_PAUSED>
//        <PLAYBACK_ORDER>3</PLAYBACK_ORDER>
//    </state>
//    <playlist>	
//        <PLAYLIST_PLAYING_ITEMS_COUNT>3</PLAYLIST_PLAYING_ITEMS_COUNT>
//        <PLAYLIST_PAGE>0</PLAYLIST_PAGE>
//        <PLAYLIST_ITEMS_PER_PAGE>10</PLAYLIST_ITEMS_PER_PAGE>
//        <PLAYLIST_TOTAL_TIME>11:09</PLAYLIST_TOTAL_TIME>
//        <PLAYLIST_ITEM_PLAYING>2</PLAYLIST_ITEM_PLAYING>
//    </playlist>
//    <item>
//        <track>Brother, O Brother,Bob Evans,09</track>
//        <album>Goodnight, Bull Creek!,Bob Evans,0</album>
//        <path>G:&#92;Music&#92;Bob Evans&#92;Goodnight, Bull Creek!&#92;009 Bob Evans - Brother, O Brother.flac</path>
//        <ITEM_PLAYING_POS>183</ITEM_PLAYING_POS>
//        <ITEM_PLAYING_LEN>242</ITEM_PLAYING_LEN>
//    </item>
//</foobar2000>