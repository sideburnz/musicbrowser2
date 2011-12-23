using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Util;
using MusicBrowser.Engines.Transport;
using Microsoft.MediaCenter.UI;
using System.Xml;

namespace MusicBrowser.Models
{
    // now playing page for Foobar2000 transport

    public enum PlayState
    {
        Undefined,
        Playing,
        Paused
    }

    public enum PlaybackStyles
    {
        Shuffle,
        RepeatTrack,
        RepeatPlaylist,
        Default
    }

    public class PlaylistItem : BaseModel
    {
        string Artist;
        string Track;
        string Duration;
        int Index;
    }

    public enum FoobarInternalPlaybackStyles
    {
        Default = 0,
        Repeat_playlist = 1,
        Repeat_track = 2,
        Random = 3,
        Shuffle_tracks = 4,
        Shuffle_albums = 5,
        Shuffle_folders = 6
    }

    public struct PlayerInformation
    {
        public bool isPlaying;
        public bool isPaused;
        public PlaybackStyles playbackStyle;

        public int playlistSize;
        public int playlistCurrentPage;
        public int playlistPageSize;
        public IEnumerable<PlaylistItem> playlistPageData;
        public string playlistDuration;
        public int playlistItemPlaying;

        public string playingTrackArtist;
        public string playingTrackName;
        public string playingTrackNumber;
        public string playingTrackAlbumArtist;
        public string playingTrackAlbumName;
        public string playingTrackPath;

        public int playingTrackProgress;
        public int playingTrackLength;
    }

    public class Foobar2000 : Foobar2000Transport
    {
        private readonly Timer _timer;
        
        public Foobar2000()
        {
            _timer = new Timer(this)
            {
                Interval = 1000,
                AutoRepeat = true,
                Enabled = true
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
                    FirePropertyChanged("PlaybackStyle");
                }
            }
        }

        //
        List<PlaylistItem> _playlistPageData = new List<PlaylistItem>();
        public List<PlaylistItem> PlaylistPageData
        {
            get
            {
                return _playlistPageData;
            }
            private set
            {
                if (value == null)
                {
                    _playlistPageData.Clear();
                    FirePropertyChanged("PlaylistPageData");
                    return;
                }
                if (_playlistPageData.GetHashCode() != value.GetHashCode())
                {
                    _playlistPageData.Clear();
                    _playlistPageData.AddRange(value);
                    FirePropertyChanged("PlaylistPageData");
                }
            }
        }
        private int _playlistpage = 0;
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
                    FirePropertyChanged("PlaylistPage");
                }
            }
        }
        private int _playlistPages = 0;
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
                    FirePropertyChanged("PlaylistPages");
                }
            }
        }
        private int _playlistLength = 0;
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
                    FirePropertyChanged("PlaylistLength");
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
                    FirePropertyChanged("PlaylistDuration");
                }
            }
        }

        // context information
        private baseEntity _currentTrack = null;
        public baseEntity CurrentTrack 
        {
            get
            {
                if (_currentTrack != null)
                {
                    return _currentTrack;
                }
                return new Track() { Path = "placeholder" };
            }
            protected set
            {
                if (CurrentTrack.CacheKey != value.CacheKey)
                {
                    _currentTrack = value;
                    FirePropertyChanged("CurrentTrack");
                }
            }
        }
        private int _position = 0;
        public int Position 
        { 
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    FirePropertyChanged("Position");
                }
            }
        }
        private PlayState _state = PlayState.Undefined;
        PlayState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    FirePropertyChanged("State");
                }
            }
        }

        public void GotoPlaylistPage(int pageNum)
        {
            ExecuteCommand("SwitchPlaylistPage=" + pageNum);
        }

        void Seek(int location)
        {
            ExecuteCommand("SeekDelta=" + _position + location);
        }

        private PlayerInformation ProcessResponse(string xml)
        {
            PlayerInformation result = new PlayerInformation();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml);
                result.isPaused = (Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/state/IS_PAUSED", "0") == "1");
                result.isPlaying = (Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/state/IS_PLAYING", "0") == "1");

                int i = 0;
                if (Int32.TryParse(Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/state/PLAYBACK_ORDER", "0"), out i))
                {
                    FoobarInternalPlaybackStyles fbs = (FoobarInternalPlaybackStyles)i;
                    switch (fbs)
                    {
                        case FoobarInternalPlaybackStyles.Default:
                            {
                                result.playbackStyle = PlaybackStyles.Default;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.Random:
                        case FoobarInternalPlaybackStyles.Shuffle_albums:
                        case FoobarInternalPlaybackStyles.Shuffle_folders:
                        case FoobarInternalPlaybackStyles.Shuffle_tracks:
                            {
                                result.playbackStyle = PlaybackStyles.Shuffle;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.Repeat_playlist:
                            {
                                result.playbackStyle = PlaybackStyles.RepeatPlaylist;
                                break;
                            }
                        case FoobarInternalPlaybackStyles.Repeat_track:
                            {
                                result.playbackStyle = PlaybackStyles.RepeatTrack;
                                break;
                            }
                    }
                }
                else
                {
                    result.playbackStyle = PlaybackStyles.Default;
                }



                i = 0;
                if (Int32.TryParse(Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/item/ITEM_PLAYING_POS", "0"), out i))
                {
                    result.playingTrackProgress = i;
                }
                else
                {
                    result.playingTrackProgress = 0;
                }

                result.playingTrackPath = Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/item/path", String.Empty);

                i = 0;
                if (Int32.TryParse(Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/playlist/PLAYLIST_PLAYING_ITEMS_COUNT", "0"), out i))
                {
                    result.playlistSize = i;
                }
                else
                {
                    result.playlistSize = 0;
                }

                result.playlistDuration = Util.Helper.ReadXmlNode(xmldoc, "/foobar2000/playlist/PLAYLIST_TOTAL_TIME", "0:00");

            }
            catch { }

            return result;
        }

        private static string _path = String.Empty;
        public void OnTick()
        {
            string xml = ExecuteCommand("RefreshPlayingInfo");

            if (!String.IsNullOrEmpty(xml))
            {
                PlayerInformation info = ProcessResponse(xml);
                PlaybackStyle = info.playbackStyle;
                if (info.playlistPageData != null)
                {
                    PlaylistPageData = info.playlistPageData.ToList<PlaylistItem>();
                }
                else
                {
                    PlaylistPageData = null;
                }
                PlaylistPage = info.playlistCurrentPage;
                PlaylistPages = (int)Math.Ceiling((double)info.playlistSize / (double)info.playlistPageSize);

                Position = info.playingTrackProgress;

                if (_path != info.playingTrackPath)
                {
                    CurrentTrack = EntityFactory.GetItem(info.playingTrackPath);
                    _path = info.playingTrackPath;
                }

                if (info.isPaused) { State = PlayState.Paused; }
                else if (info.isPlaying) { State = PlayState.Playing; }
                else { State = PlayState.Undefined; }

                PlaylistDuration = info.playlistDuration;
                PlaylistLength = info.playlistSize;
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
//        <PLAYLIST><tr onclick="a('0')" class="o">Bob Evans,Someone So Much,3:35|</tr><tr onclick="a('1')" class="e prev">Bob Evans,Hand Me Downs,3:32|</tr><tr onclick="a('2')" class="npr  focus" id="nowplaying">Bob Evans,Brother, O Brother,4:02|</tr></PLAYLIST>
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