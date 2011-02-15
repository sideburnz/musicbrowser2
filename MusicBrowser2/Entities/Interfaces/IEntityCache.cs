using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Entities.Interfaces
{
    public interface IEntityCache
    {
        //CRUD operations

        void Delete(string key);
        IEntity Read(string key);
        void Update(string key, IEntity entity);
        
        // support functions

        bool Exists(string key);
        bool IsValid(string key, params DateTime[] comparisons);

        // stats

        int Hits { get; }
        int Misses { get; }
        int Size { get; }

    }
}
