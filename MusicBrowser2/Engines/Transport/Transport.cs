using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Transport
{
    static class Transport
    {
        private static ITransport _transport;

        public static ITransport GetTransport()
        {
            if (_transport == null)
            {
                switch (Util.Config.GetInstance().GetStringSetting("Engine").ToLower())
                {
                    case "foobar2000":
                        {
                            _transport = new Foobar2000Transport();
                            break;
                        }
                    case "vlc":
                        {
                            _transport = new VLCTransport();
                            break;
                        }
                    default:
                        {
                            _transport = new MediaCentreTransport();
                            break;
                        }
                }
            }
            return _transport;
        }
    }
}
