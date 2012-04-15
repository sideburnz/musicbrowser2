using System;

namespace MusicBrowser.Entities
{
    // empty implementation of the playstate
    public class BlankPlayState : IPlayState
    {
        public long TimesPlayed
        {
            get { return 0; }
            set { return; }
        }

        public long Progress
        {
            get { return 0; }
            set { return; }
        }

        public DateTime FirstPlayed
        {
            get { return DateTime.MinValue; }
            set { return; }
        }

        public DateTime LastPlayed
        {
            get { return DateTime.MinValue; }
            set { return; }
        }

        public bool Played
        {
            get { return false; }
            set { return; }
        }

        public void Play()
        {
            return;
        }
    }
}
