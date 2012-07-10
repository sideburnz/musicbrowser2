﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBrowser.Engines.ViewState;

namespace MusicBrowser.Entities
{
    [DataContract]
    class MovieSeries : VideoCollection
    {
        public override string Information
        {
            get
            {
                return CalculateInformation("Movie Series", "Movie");
            }
        }

        public override IViewState ViewState
        {
            get
            {
                IViewState view = base.ViewState;
                view.DefaultSort = "[ReleaseDate#:sort]";
                return view;
            }
        }

        public override List<string> SortFields
        {
            get
            {
                return new List<string>
                           {
                               "ReleaseDate",
                               "Title",
                               "Filename"
                           };
            }
        }
    }
}
