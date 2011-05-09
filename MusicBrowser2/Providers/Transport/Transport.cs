using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Providers.Transport
{
    static class Transport
    {
        private static ITransport _transport = null;

        public static ITransport getTransport()
        {
            if (_transport == null)
            {
                switch (Util.Config.getInstance().getSetting("Engine").ToLower())
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
