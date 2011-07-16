using System;
using MusicBrowser.Providers.CD;

namespace MusicBrowser.Entities.Kinds
{
    class Disc : IEntity
    {
        private readonly char _letter;
        private readonly CDDrive _drive;

        public Disc(char letter)
        {
            _letter = letter;
            _drive = new CDDrive();

            _drive.Open(_letter);
            if (_drive.Refresh())
            {
                if (_drive.GetNumAudioTracks() > 0)
                {
                    OnCDInserted();
                }
            }
            _drive.UnLockCD();
            _drive.Close();
            Title = "Audio CD (" + _letter + ":)";
            DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imageDisc";
            CalculateValues();
        }

        public override string Path
        {
            get
            {
                return _letter + ":\\";
            }
        }

        public override bool Playable
        {
            get
            {
                return true;
            }
        }

        private void OnCDInserted()
        {
            int tracks = _drive.GetNumAudioTracks();
            int T = _drive.GetNumTracks();
            int length = 0;
            string duration;

            for (int i = 1; i <= T; i++)
            {
                if (_drive.IsAudioTrack(i))
                {
                    length += _drive.GetSeconds(i);
                }
            }

            TimeSpan t = TimeSpan.FromSeconds(length);
            if (t.Hours == 0)
            {
                duration = string.Format("{0}:{1:D2}", (Int32)Math.Floor(t.TotalMinutes), t.Seconds);
            }
            else
            {
                duration = string.Format("{0}:{1:D2}:{2:D2}", (Int32)Math.Floor(t.TotalHours), t.Minutes, t.Seconds);
            }


            base.ShortSummaryLine1 = "Disc (" + tracks + " Tracks  " + duration + ")";

            //fire off background task to get info off CDDB
            FirePropertyChanged("ShortSummaryLine1");
        }

        public char Letter
        {
            get { return _letter; }
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Disc; }
        }
    }
}
