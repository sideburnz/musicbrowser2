using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MusicBrowser.Models;
using ServiceStack.Text;
using System.Text.RegularExpressions;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Track : Music
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageTrack"; }
        }

        public override string DefaultFormat
        {
            get { return "[Track#] - [Title]"; }
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
        public DateTime ReleaseDate 
        {
            get { return _releaseDate; }
            set
            {
                if (value != _releaseDate)
                {
                    _releaseDate = value;
                    DataChanged("ReleaseDate");
                }
            }
        }
        private DateTime _releaseDate;
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
        public int Duration 
        {
            get { return _duration; }
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    DataChanged("Duration");
                }
            }
        }
        private int _duration;
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
        public IEnumerable<string> Genres 
        {
            get 
            {
                if (_genres == null)
                {
                    return new List<string>();
                }
                return _genres;
            }
            set
            {
                _genres = value;
                DataChanged("Genres");
            }
        }
        private IEnumerable<string> _genres;



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



        public override string Information
        {
            get
            {
                if (!String.IsNullOrEmpty(Artist))
                {
                    return TokenSubstitution("[artist]  ([duration])");
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
                    case "ReleaseDate":
                    case "releasedate":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("dd mmm yyyy"));
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
                    case "ReleaseDate:sort":
                    case "releaserate:sort":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000")) 
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.ToString("yyyymmdd hhMMss")); 
                            break; 
                        }
                        output = output.Replace("[" + token + "]", ""); 
                        break;
                    case "ReleaseYear":
                    case "releaseyear":
                    case "ReleaseYear:sort":
                    case "releaseyear:sort":
                        if (ReleaseDate > DateTime.Parse("01-JAN-1000"))
                        {
                            output = output.Replace("[" + token + "]", ReleaseDate.Year.ToString());
                            break;
                        }
                        output = output.Replace("[" + token + "]", "");
                        break;
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

                    case "Duration":
                    case "duration":
                        TimeSpan t = TimeSpan.FromSeconds(Duration);
                        string length;
                        if (t.Hours == 0)
                        {
                            length = string.Format("{0}:{1:D2}", (Int32)Math.Floor(t.TotalMinutes), t.Seconds);
                        }
                        else
                        {
                            length = string.Format("{0}:{1:D2}:{2:D2}", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds);
                        }
                        output = output.Replace("[" + token + "]", length);
                        break;
                    case "Duration:sort":
                    case "duration:sort":
                         output = output.Replace("[" + token + "]", TrackNumber.ToString("D6")); break;
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
