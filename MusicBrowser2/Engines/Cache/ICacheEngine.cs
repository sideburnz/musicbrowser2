using System;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using System.Collections.Generic;

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

        IEnumerable<string> Search(string kind, string predicate);
    }
}
