using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// This interface abstracts music players to allow other MB2 to use non-MCE music players
// Transports should be singletons

namespace MusicBrowser.Providers.Transport
{
    public struct TrackInfo
    {
        string Album;
        string Artist;
        string Track;
        int Length;
        DateTime Release;
        int TrackNumber;
    }

    public enum PlayState
    {
        Undefined,
        Playing,
        Paused
    }

    public interface ITransport
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
        TrackInfo CurrentTrackInfo { get; }
        int Progress { get; }
        PlayState State { get; }

        // events
        //void OnTrackChanged();
        //void OnTrackProgress();
        event EventHandler TrackChanged;
        event EventHandler TrackProgress;

    }
}
