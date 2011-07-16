using System;
using System.Text;

namespace MusicBrowser.Entities.Kinds
{
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

        public string Track
        {
            get
            {
                if (Util.Config.GetInstance().GetBooleanSetting("PutDiscInTrackNo"))
                {
                    if (TrackNumber < 1) 
                    { 
                        return string.Empty; 
                    }
                    if (DiscNumber > 0) 
                    {
                        return DiscNumber + "." + TrackNumber;
                    }
                    return TrackNumber.ToString(); 
                }
                if (TrackNumber > 0) { return TrackNumber.ToString(); }
                return string.Empty;
            }
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

        public override void CalculateValues()
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
            base.CalculateValues();
        }

        public override IEntity Parent
        {
            get { return base.Parent; }
            set
            {
                base.Parent = value;
                if (value.Kind.Equals(EntityKind.Album))
                {
                    if (String.IsNullOrEmpty(AlbumName)) { AlbumName = value.AlbumName; }
                    Dirty = true;
                }
                if (value.Kind.Equals(EntityKind.Album))
                {
                    if (String.IsNullOrEmpty(AlbumArtist)) { AlbumArtist = value.ArtistName; }
                    Dirty = true;
                }
                if (Util.Config.GetInstance().GetBooleanSetting("UseFolderImageForTracks"))
                {
                    IconPath = value.IconPath;
                }
            }
        }
    }
}
