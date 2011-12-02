using System;

namespace MusicBrowser.Interfaces
{
    public interface ICacheEngine
    {
        void Delete(string key);
        byte[] Fetch(string key);
        void Update(string key, byte[] value);

        bool Exists(string key);

        void Scavenge();
        void Clear();
    }
}
