using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities.Interfaces;

namespace MusicBrowser.Entities.Kinds
{
    class Unknown : IEntity
    {
        public Unknown()
        {
            base.Title = "[unknown]";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Unknown; }
        }
    }
}
