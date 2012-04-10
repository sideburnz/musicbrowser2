
namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        void IPlugIn.Register()
        {
            Metadata.Providers.RegisterProvider(new MusicBrowser.Engines.Metadata.HTBackdropMetadataProvider());
        }
    }
}
