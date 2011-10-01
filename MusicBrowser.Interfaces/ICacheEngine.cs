using System;

namespace MusicBrowser.Interfaces
{
    public interface ICacheEngine
    {
        void Delete(string key);
        string Fetch(string key);
        void Update(string key, string value);

        bool Exists(string key);

        void Scavenge();
    }
}
