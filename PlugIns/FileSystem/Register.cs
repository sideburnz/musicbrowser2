﻿
namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        void IPlugIn.Register()
        {
            Metadata.Providers.RegisterProvider(new Metadata.FileSystemMetadataProvider());
            Metadata.Providers.RegisterProvider(new Metadata.InheritanceMetadataProvider());
        }
    }
}
