using System.Collections.Generic;

namespace MusicBrowser.Engines.Metadata
{
    public static class Providers
    {
        private static readonly List<IProvider> _Providers = new List<IProvider>() { new MediaInfoProvider() };

        public static void RegisterProvider(IProvider provider)
        {
            _Providers.Add(provider);
        }

        public static IEnumerable<IProvider> ProviderList
        {
            get
            {
                return _Providers;
            }
        }
    }
}
