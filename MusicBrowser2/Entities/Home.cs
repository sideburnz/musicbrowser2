using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Home : Virtual
    {
        public override string Title
        {
            get { return "MusicBrowser 2"; }
            set { }
        }

        public override string DefaultView
        {
            get { return "Thumb"; }
        }

        public override string DefaultSort
        {
            get { return "[SortOrder:sort]"; }
        }

        public override string CacheKey
        {
            get { return "home"; }
        }

    }
}
