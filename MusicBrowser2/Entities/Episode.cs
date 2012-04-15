using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Episode : Video
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageVideo"; }
        }

        protected override string DefaultFormat
        {
            get
            {
                string title = Util.Config.GetInstance().GetStringSetting("Entity.Episode.Format");
                if (string.IsNullOrEmpty(title))
                {
                    return "[Episode#] - [Title]";
                }
                return base.DefaultFormat;
            }
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

        #region overrides
        public override string Information
        {
            get
            {
                if (!String.IsNullOrEmpty(ShowName))
                {
                    return ShowName + ", Season " + SeasonNumber + "  " + base.Information + "  (" + TokenSubstitution("[ReleaseYear]") + ")";
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
        #endregion
    }
}
