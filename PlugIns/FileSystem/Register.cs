using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        void IPlugIn.Register()
        {
            Metadata.Providers.RegisterProvider(new MusicBrowser.Engines.Metadata.FileSystemMetadataProvider());
            Metadata.Providers.RegisterProvider(new MusicBrowser.Engines.Metadata.InheritanceMetadataProvider());
        }
    }
}
