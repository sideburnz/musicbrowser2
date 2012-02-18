using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Virtuals
{
    public interface IView
    {
        string Title { get; }
        EntityCollection Items { get; }
        string Sort { get; }
        bool SortAscending { get; }
    }
}
