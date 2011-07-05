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
                    if (!Properties.ContainsKey("track")) 
                    { 
                        return string.Empty; 
                    }
                    if (Properties.ContainsKey("disc")) 
                    {
                        if (!Properties["disc"].Equals("0"))
                        {
                            return Properties["disc"] + "." + Properties["track"];
                        }
                        
                    }
                    return Properties["track"]; 
                }
                if (Properties.ContainsKey("track")) { return Properties["track"]; }
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

        //TODO: calc ShortSummaryLine2 [hand off the the appropriate metadata provider]
        public override void CalculateValues()
        {
            StringBuilder sb = new StringBuilder();
            if (Duration > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(Duration);
                sb.Append (string.Format("{0}:{1:D2}  ", t.Minutes, t.Seconds));
            }
            if (Properties.ContainsKey("resolution")) { sb.Append(Properties["resolution"] + "  "); }
            if (Properties.ContainsKey("channels")) { sb.Append(Properties["channels"] + "  "); }
            if (Properties.ContainsKey("samplerate")) { sb.Append(Properties["samplerate"] + "  "); }
            if (Properties.ContainsKey("codec")) { sb.Append(Properties["codec"] + "  "); }

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
                    SetProperty("album", value.Title, false);
                    Dirty = true;
                }
                if (value.Kind.Equals(EntityKind.Album) && value.Properties.ContainsKey("artist"))
                {
                    SetProperty("artist", value.Properties["artist"], false);
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
