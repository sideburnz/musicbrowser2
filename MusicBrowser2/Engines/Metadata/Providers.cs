using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Engines.Metadata
{
    public static class Providers
    {
        private static List<IProvider> _providers = new List<IProvider>();

        public static void RegisterProvider(IProvider provider)
        {
            _providers.Add(provider);
        }

        public static IEnumerable<IProvider> ProviderList
        {
            get
            {
                return _providers;
            }
        }
    }
}
