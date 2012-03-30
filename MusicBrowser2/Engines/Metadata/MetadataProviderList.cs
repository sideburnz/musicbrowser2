using System;
using System.Collections.Generic;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Engines.Metadata
{
    class MetadataProviderList : IBackgroundTaskable
    {
        private static readonly IEnumerable<IProvider> Providers = Metadata.Providers.ProviderList;

        public static void ProcessEntity(baseEntity entity, bool forced)
        {
#if DEBUG
            LoggerEngineFactory.Verbose("ProcessEntity(" + entity.Path + ", <providers>, " + forced + ")", "start");
#endif
            // only waste time triggering cache updates if the content has changed
            bool requiresUpdate = false;

            foreach (IProvider provider in Providers)
            {
                try
                {
                    if (!provider.CompatibleWith(entity)) { continue; }
                    DateTime lastAccess = entity.MetadataStamps.ContainsKey(provider.FriendlyName()) ? entity.MetadataStamps[provider.FriendlyName()] : DateTime.MinValue;
                    if (!forced && !provider.isStale(lastAccess)) { continue; }

                    // execute the payload
                    ProviderOutcome outcome = provider.Fetch(entity);

                    if (outcome == ProviderOutcome.Success)
                    {
                        requiresUpdate = true;
                        entity.MetadataStamps[provider.FriendlyName()] = DateTime.Now;
                    }
                    else if (outcome != ProviderOutcome.NoData) // no data is a warning, ignore it and move on
                    {
                        entity.MetadataStamps[provider.FriendlyName()] = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    LoggerEngineFactory.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType(), entity.Path), e));
#endif
                }
            }
            if (requiresUpdate)
            {
                entity.UpdateCache();
            }
        }

        private readonly baseEntity _entity;
        private readonly bool _forced;

        public MetadataProviderList(baseEntity entity)
        {
            _entity = entity;
            _forced = false;
        }

        public MetadataProviderList(baseEntity entity, bool forced)
        {
            _entity = entity;
            _forced = forced;
        }

        #region IBackgroundTaskable Members

        public string Title
        {
            get { return "MetadataProviderList(" + _entity.Path + ")"; }
        }

        public void Execute()
        {
            try
            {
                ProcessEntity(_entity, _forced);
            }
            catch (Exception e)
            {
                LoggerEngineFactory.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
            }

        }

        #endregion
    }
}
