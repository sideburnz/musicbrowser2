﻿
namespace MusicBrowser.Providers.Transport
{
    static class Transport
    {
        private static ITransport _transport;

        public static ITransport GetTransport()
        {
            if (_transport == null)
            {
                switch (Util.Config.GetInstance().GetSetting("Engine").ToLower())
                {
                    case "foobar2000":
                        {
                            _transport = new Foobar2000Transport();
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
