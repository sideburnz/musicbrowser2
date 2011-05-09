using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Providers.Metadata
{
    public interface IMetadataProvider
    {
        IEntity Fetch(IEntity entity);
    }
}
