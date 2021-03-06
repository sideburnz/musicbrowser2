﻿using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Metadata
{
    public abstract class baseMetadataProvider : IProvider
    {
        protected int MinDaysBetweenHits { private get; set; }
        protected int MaxDaysBetweenHits { private get; set; }
        protected int RefreshPercentage { private get; set; }
        protected string Name { private get; set; }

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public abstract bool CompatibleWith(baseEntity dto);

        public string FriendlyName()
        {
            return Name;
        }

        public bool isStale(DateTime lastAccess)
        {
            // if it's never refreshed, refresh it
            if (lastAccess < DateTime.Parse("01-JAN-1000")) { return true; }

            // if it's less then the min, don't refresh if it's older than the max then do refresh
            int dataAge = (DateTime.Today.Subtract(lastAccess).Days);
            if (dataAge <= MinDaysBetweenHits) { return false; }
            if (dataAge >= MaxDaysBetweenHits) { return true; }

            // otherwise refresh randomly
            return (Rnd.Next(100) >= RefreshPercentage);
        }

        public ProviderOutcome Fetch(baseEntity dto)
        {
#if DEBUG
            Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif
            ProviderOutcome ret;

            try
            {
                // ask the killer questions
                if (!AskKillerQuestions(dto))
                {
                    ret = ProviderOutcome.SystemError;
                    return ret;
                }

                // log the statistic for telemetry
                MusicBrowser.Providers.Telemetry.Hit(Name + ".hit");

                // do the payload
                ret = DoWork(dto);
            }
            catch
            {
                ret = ProviderOutcome.SystemError;
            }
            return ret;
        }

        protected abstract ProviderOutcome DoWork(baseEntity dto);

        protected abstract bool AskKillerQuestions(baseEntity dto);
    }
}