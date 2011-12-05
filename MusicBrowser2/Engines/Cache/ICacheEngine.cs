using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public interface ICacheEngine
    {
        void Delete(string key);
        baseEntity Fetch(string key);
        void Update(baseEntity entity);

        bool Exists(string key);

        void Scavenge();
        void Clear();
    }
}
