using System;
using System.Collections.Generic;

// This interface abstracts music players to allow other MB2 to use non-MCE music players
// Transports should be singletons

namespace MusicBrowser.Interfaces
{
    public enum PlayState
    {
        Undefined,
        Playing,
        Paused
    }

    public interface ITransportEngine
    {
        // transport controls
        void Play(bool queue, string file);
        void Play(bool queue, IEnumerable<string> files);
        void PlayDisc(string drive);
        void PlayPause();
        void Stop();
        void Next();
        void Previous();

        void Close();

        // context information
        //TrackInfo CurrentTrackInfo { get; }
        //int Progress { get; }
        PlayState State { get; }

        // events
        //void OnTrackChanged();
        //void OnTrackProgress();
        //event EventHandler TrackChanged;
        //event EventHandler TrackProgress;

    }
}
