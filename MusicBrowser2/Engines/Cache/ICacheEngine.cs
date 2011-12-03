using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public interface ICacheEngine
    {
        void Delete(string key);
        Entity Fetch(string key);
        void Update(Entity entity);

        bool Exists(string key);

        void Scavenge();
        void Clear();
    }
}
