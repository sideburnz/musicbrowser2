using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.MediaCenter.UI;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Track : Music
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageTrack"; }
        }

        [DataMember]
        public string Artist 
        {
            get { return _artist; }
            set
            {
                if (value != _artist)
                {
                    _artist = value;
                    DataChanged("Artist");
                }
            }
        }
        private string _artist;
        [DataMember]
        public string Album
        {
            get { return _album; }
            set
            {
                if (value != _album)
                {
                    _album = value;
                    DataChanged("Album");
                }
            }
        }
        private string _album;
        [DataMember]
        public string AlbumArtist
        {
            get { return _albumArtist; }
            set
            {
                if (value != _albumArtist)
                {
                    _albumArtist = value;
                    DataChanged("AlbumArtist");
                }
            }
        }
        private string _albumArtist;
        [DataMember]
        public int DiscNumber 
        {
            get { return _discNumber; }
            set
            {
                if (value != _discNumber)
                {
                    _discNumber = value;
                    DataChanged("DiscNumber");
                }
            }
        }
        private int _discNumber;
        [DataMember]
        public int TrackNumber 
        {
            get { return _trackNumber; }
            set
            {
                if (value != _trackNumber)
                {
                    _trackNumber = value;
                    DataChanged("TrackNumber");
                }
            }
        }
        private int _trackNumber;
        [DataMember]
        public string Codec 
        {
            get { return _codec; }
            set
            {
                if (value != _codec)
                {
                    _codec = value;
                    DataChanged("Codec");
                }
            }
        }
        private string _codec;
        [DataMember]
        public string MusicBrainzID 
        {
            get { return _musicbrainzID; }
            set
            {
                if (value != _musicbrainzID)
                {
                    _musicbrainzID = value;
                    DataChanged("MusicBrainzID");
                }
            }
        }
        private string _musicbrainzID;
        [DataMember]
        public string Genre 
        {
            get 
            {
                return _genre;
            }
            set
            {
                _genre = value;
                DataChanged("Genre");
            }
        }
        private string _genre;

        [DataMember]
        public int Listeners { get; set; }
        [DataMember]
        public int LastFMPlayCount { get; set; }
        [DataMember]
        public string SampleRate
        {
            get { return _samplerate; }
            set
            {
                if (value != _samplerate)
                {
                    _samplerate = value;
                    DataChanged("SampleRate");
                }
            }
        }
        private string _samplerate;
        [DataMember]
        public string Channels
        {
            get { return _channels; }
            set
            {
                if (value != _channels)
                {
                    _channels = value;
                    DataChanged("Channels");
                }
            }
        }
        private string _channels;
        [DataMember]
        public string Resolution
        {
            get { return _resolution; }
            set
            {
                if (value != _resolution)
                {
                    _resolution = value;
                    DataChanged("Resolution");
                }
            }
        }
        private string _resolution;

        public override List<Image> CodecIcons 
        {
            get 
            { 
                List<Image> ret = new List<Image>
                                      {
                                          Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/codec_" + Codec),
                                          Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/bit_" + Resolution),
                                          Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/rate_" + SampleRate),
                                          Util.Helper.GetImage("resx://MusicBrowser/MusicBrowser.Resources/channels_" + Channels.ToLower())
                                      };

                return ret;
            } 
        }

        public string DurationString
        {
            get { return TokenSubstitution("[Duration]"); }
        }

        public override string Information
        {
            get
            {
                if (!String.IsNullOrEmpty(Artist))
                {
                    return TokenSubstitution("[artist]  ([duration])  ");
                }
                return base.Information;
            }
        }

        public override string TokenSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "Artist":
                    case "artist":
                        output = output.Replace("[" + token + "]", Artist); break;
                    case "Artist:sort":
                    case "artist:sort":
                        output = output.Replace("[" + token + "]", HandleIgnoreWords(Artist)); break;
                    case "Album":
                    case "album":
                    case "Album:sort":
                    case "album:sort":
                        output = output.Replace("[" + token + "]", Album); break;
                    case "AlbumArtist":
                    case "albumartist":
                        output = output.Replace("[" + token + "]", AlbumArtist); break;
                    case "AlbumArtist:sort":
                    case "albumartist:sort":
                        output = output.Replace("[" + token + "]", HandleIgnoreWords(AlbumArtist)); break;
                    case "Disc#":
                    case "disc#":
                    case "Disc#:sort":
                    case "disc#:sort":
                        output = output.Replace("[" + token + "]", DiscNumber.ToString()); break;
                    case "Track#":
                    case "track#":
                    case "Track#:sort":
                    case "track#:sort":
                        if (Util.Config.GetInstance().GetBooleanSetting("PutDiscInTrackNo") && DiscNumber > 0)
                        {
                            output = output.Replace("[" + token + "]", DiscNumber.ToString() + "." + TrackNumber.ToString("D2")); break;
                        }
                        else
                        {
                            output = output.Replace("[" + token + "]", TrackNumber.ToString("D2")); break;
                        }
                    case "Codec":
                    case "codec":
                    case "Codec:sort":
                    case "codec:sort":
                        output = output.Replace("[" + token + "]", Codec); break;
                    case "samplerate":
                    case "SampleRate":
                    case "samplerate:sort":
                    case "SampleRate:sort":
                        output = output.Replace("[" + token + "]", SampleRate); break;
                    case "resolution":
                    case "Resolution":
                    case "resolution:sort":
                    case "Resolution:sort":
                        output = output.Replace("[" + token + "]", Resolution); break;
                    case "channels":
                    case "Channels":
                    case "channels:sort":
                    case "Channels:sort":
                        output = output.Replace("[" + token + "]", Channels); break;
                    case "lastfmplaycount":
                    case "LastfmPlayCount":
                        output = output.Replace("[" + token + "]", LastFMPlayCount.ToString()); break;
                    case "lastfmplaycount:sort":
                    case "LastfmPlayCount:sort":
                        output = output.Replace("[" + token + "]", LastFMPlayCount.ToString("D9")); break;
                }
            }

            return base.TokenSubstitution(output);
        }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
