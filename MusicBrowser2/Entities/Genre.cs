﻿using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Genre : Folder
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageGenre"; }
        }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Artist", "Album", "Track");
            }
        }
    }
}
