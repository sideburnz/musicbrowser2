using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class FastProviderMarker : IDataProvider
    {
        private const string Name = "FastProviderMarker";

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            dto.Outcome = DataProviderOutcome.Success;
            Statistics.Hit(Name + ".hit");
            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return true;
        }

        public bool isStale(DateTime lastAccess)
        {
            // this should only need to run once
            return (lastAccess.AddDays(365) < DateTime.Now);
        }

        public ProviderType Type
        {
            get { return ProviderType.Core; }
        }
    }
}
