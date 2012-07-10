namespace MusicBrowser.Engines.Transport
{
    static class TransportEngineFactory
    {
        private static ITransportEngine _transport;

        public static ITransportEngine GetEngine()
        {
            if (_transport == null)
            {
                switch (Util.Config.GetStringSetting("Player.Engine").ToLower())
                {
                    case "foobar2000":
                        {
                            _transport = new Foobar2000Transport();
                            _transport.Open();
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
