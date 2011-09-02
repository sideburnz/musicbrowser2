using System;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities.Kinds
{
    [DataContract]
    [KnownType(typeof(Song))]
    public class Song : IEntity
    {
        public Song()
        {
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageSong";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Song; }
        }

        public override bool Playable
        {
            get
            {
                return true;
            }
        }

        public override string ShortSummaryLine1
        {
            get
            {
                if (string.IsNullOrEmpty(base.ShortSummaryLine1))
                {
                    return Kind.ToString();
                }
                return base.ShortSummaryLine1;
            }
            set { base.ShortSummaryLine1 = value; }
        }

        public override void UpdateValues()
        {
            StringBuilder sb = new StringBuilder();
            if (Duration > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(Duration);
                sb.Append (string.Format("{0}:{1:D2}  ", t.Minutes, t.Seconds));
            }
            if (!String.IsNullOrEmpty(Resolution)) { sb.Append(Resolution + "  "); }
            if (!String.IsNullOrEmpty(Channels)) { sb.Append(Channels + "  "); }
            if (!String.IsNullOrEmpty(SampleRate)) { sb.Append(SampleRate + "  "); }
            if (!String.IsNullOrEmpty(Codec)) { sb.Append(Codec + "  "); }

            if (sb.Length > 0) { base.ShortSummaryLine1 = "Song  (" + sb.ToString().Trim() + ")"; }
            base.UpdateValues();
        }
    }
}
