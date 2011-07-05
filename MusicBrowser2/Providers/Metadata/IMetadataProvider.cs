using MusicBrowser.Entities;

namespace MusicBrowser.Providers.Metadata
{
    public interface IMetadataProvider
    {
        IEntity Fetch(IEntity entity);
    }
}
