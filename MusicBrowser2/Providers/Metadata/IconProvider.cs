using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class IconProvider : IDataProvider
    {
        private const string Name = "IconProvider";

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            Logging.Logger.Debug(Name + ": " + dto.Path);

            #region killer questions

            if (!Directory.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }

            #endregion


            /* 
             * if the IBN folder exists, look for this genre in the folder and select a random image from it 
             */

            dto.Outcome = DataProviderOutcome.Success;
            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return ((type.ToLower() == "album") || (type.ToLower() == "artist") || (type.ToLower() == "genre"));
        }

        public bool isStale(DateTime lastAccess)
        {
            // refresh weekly
            return (lastAccess.AddDays(7) > DateTime.Now);
        }
    }
}
