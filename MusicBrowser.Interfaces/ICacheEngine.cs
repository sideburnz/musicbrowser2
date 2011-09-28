using System;

namespace MusicBrowser.Interfaces
{
    public interface ICacheEngine
    {
        void Delete(string key);
        string FetchIfFresh(string key, DateTime comparer);
        void Update(string key, string value);

        bool Exists(string key);

        void Scavenge();
    }
}
