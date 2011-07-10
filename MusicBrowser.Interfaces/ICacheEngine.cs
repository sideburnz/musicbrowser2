using System;

namespace MusicBrowser.Interfaces
{
    public interface ICacheEngine
    {
        void Delete(string key);
        string Read(string key);
        void Update(string key, string value);

        bool Exists(string key);
        bool IsValid(string key, params DateTime[] comparisons);
    }
}
