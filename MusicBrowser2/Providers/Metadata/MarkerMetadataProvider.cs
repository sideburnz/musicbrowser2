using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class MarkerMetadataProvider : baseMetadataProvider
    {

        #region singleton
        static IDataProvider _instance;
        private static readonly object _lock = new object();
        public new static IDataProvider GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            lock (_lock)
            {
                if (_instance != null)
                {
                    _instance = new MarkerMetadataProvider();
                }
            }
            return _instance;
        }
        #endregion

        public MarkerMetadataProvider()
        {
            Name = "MarkerMetadataProvider";
            RefreshPercentage = 100;
            MinDaysBetweenHits = 0;
            MaxDaysBetweenHits = 0;
            Type = ProviderType.Core;
        }

        public override bool CompatibleWith(string type)
        {
            return true;
        }

        public override DataProviderDTO DoWork(DataProviderDTO dto)
        {
            dto.Outcome = DataProviderOutcome.Success;
            Statistics.Hit(Name + ".hit");
            return dto;
        }

        public override bool AskKillerQuestions(DataProviderDTO dto)
        {
            return true;
        } 

    }
}