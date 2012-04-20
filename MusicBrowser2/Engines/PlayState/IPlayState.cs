using System;

namespace MusicBrowser.Engines.PlayState
{
    public interface IPlayState
    {
        long TimesPlayed { get; set; }
        long Progress { get; set; }
        DateTime FirstPlayed { get; set; }
        DateTime LastPlayed { get; set; }
        bool Played { get; set; }

        /// <summary>
        /// does a few updates direct to values and then calls a single commit() to reduce DB calls
        /// </summary>
        void Play();
    }
}