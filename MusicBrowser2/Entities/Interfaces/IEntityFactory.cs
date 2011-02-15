using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities.Interfaces
{
    public interface IEntityFactory
    {
        void setCache(IEntityCache cache);
        IEntity getItem(FileSystemItem item);
        IEntity getItem(string item);
    }
}
