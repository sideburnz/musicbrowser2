using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using Microsoft.MediaCenter.UI;

namespace MusicBrowser.Entities.Kinds
{
    class Playlist : IEntity
    {
        public Playlist()
        {
            base.DefaultIconPath = "resx://MusicBrowser/MusicBrowser.Resources/imagePlaylist";
        }

        public override EntityKind Kind
        {
            get { return EntityKind.Playlist; }
        }

        public override string ShortSummaryLine1
        {
            get
            {
                if (string.IsNullOrEmpty(base.ShortSummaryLine1))
                {
                    return Kind.ToString();
                }
                return base.ShortSummaryLine1;
            }
            set { base.ShortSummaryLine1 = value; }
        }

    }
}
