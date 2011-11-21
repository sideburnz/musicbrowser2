using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    abstract class baseMetadataProvider : IDataProvider
    {
        public int MinDaysBetweenHits { get; set; }
        public int MaxDaysBetweenHits { get; set; }
        public int RefreshPercentage { get; set; }
        
        public string Name { get; set; }

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public ProviderType Type { get; set; }

        public abstract bool CompatibleWith(string type);

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

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            MusicBrowser.Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif
            DataProviderDTO ret = dto;
            dto.Outcome = DataProviderOutcome.Success;

            try
            {
                // ask the killer questions
                if (!AskKillerQuestions(ret))
                {
                    // if there's any problems report them back
                    if (ret.Outcome == DataProviderOutcome.Success)
                    {
                        ret.Outcome = DataProviderOutcome.SystemError;
                        ret.Errors = new List<string>() { String.Format("Provider: '{0}' failed killer questions but didn't give a reason") };
                    }
                    return ret;
                }

                // log the statistic for telemetry
                Statistics.Hit(Name + ".hit");

                // do the payload
                ret = DoWork(ret);
            }
            catch
            {
                ret.Outcome = DataProviderOutcome.SystemError;
                ret.Errors = new List<string>() { String.Format("Provider: '{0}' failed execution but didn't give a reason") };
            }

            return ret;
        }

        public abstract DataProviderDTO DoWork(DataProviderDTO dto);

        public static IDataProvider GetInstance()
        {
            return null;
        }
        
        public abstract bool AskKillerQuestions(DataProviderDTO dto);
    }
}