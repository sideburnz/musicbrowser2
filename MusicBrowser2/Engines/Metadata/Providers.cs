using System.Collections.Generic;

namespace MusicBrowser.Engines.Metadata
{
    public static class Providers
    {
        private static readonly List<IProvider> _providers = new List<IProvider>();

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
