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
    class Episode : Video
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageVideo"; }
        }

        public override string DefaultFormat
        {
            get { return "[Episode#] - [Title]"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        [DataMember]
        public int EpisodeNumber
        {
            get { return _episode; }
            set
            {
                if (value != _episode)
                {
                    _episode = value;
                    DataChanged("EpisodeNumber");
                }
            }
        }
        private int _episode;
        [DataMember]
        public int SeasonNumber
        {
            get { return _season; }
            set
            {
                if (value != _season)
                {
                    _season = value;
                    DataChanged("SeasonNumber");
                }
            }
        }
        private int _season;
        [DataMember]
        public string ShowName
        {
            get { return _show; }
            set
            {
                if (value != _show)
                {
                    _show = value;
                    DataChanged("ShowName");
                }
            }
        }
        private string _show;

        public override string TokenSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "Episode#":
                    case "episode#":
                        output = output.Replace("[" + token + "]", EpisodeNumber.ToString()); break;
                    case "Episode#:sort":
                    case "episode#:sort":
                        output = output.Replace("[" + token + "]", EpisodeNumber.ToString("D3")); break;
                    case "Season#":
                    case "season#":
                        output = output.Replace("[" + token + "]", SeasonNumber.ToString()); break;
                    case "Season#:sort":
                    case "season#:sort":
                        output = output.Replace("[" + token + "]", SeasonNumber.ToString("D3")); break;
                }
            }

            return base.TokenSubstitution(output);
        }
    }
}
