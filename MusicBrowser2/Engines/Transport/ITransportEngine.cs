using System.Collections.Generic;

// This interface abstracts music players to allow other MB2 to use non-MCE music players
// Transports should be singletons

namespace MusicBrowser.Engines.Transport
{
    public interface ITransportEngine
    {
        // transport controls
        void Play(bool queue, string file);
        void Play(bool queue, IEnumerable<string> files);
        void PlayPause();
        void Stop();
        void Next();
        void Previous();
        void JumpForward();
        void JumpBack();

        // instatiates and disposes of transport
        void Open();
        void Close();

        bool ShowNowPlaying();

        bool HasBespokeNowPlaying { get; }
    }
}
