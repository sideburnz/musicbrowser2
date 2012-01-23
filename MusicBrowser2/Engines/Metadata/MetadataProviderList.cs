using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Engines.Metadata
{
    class MetadataProviderList : IBackgroundTaskable
    {
        private static object obj = new object();
        private static IEnumerable<IProvider> _providers = Providers.ProviderList;

        public static void ProcessEntity(baseEntity entity, bool Forced)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("ProcessEntity(" + entity.Path + ", <providers>, " + Forced + ")", "start");
#endif
            // only waste time triggering cache updates if the content has changed
            bool requiresUpdate = false;

            foreach (IProvider provider in _providers)
            {
                try
                {
                    //DateTime lastAccess = entity.ProviderTimeStamps.ContainsKey(provider.FriendlyName()) ? entity.ProviderTimeStamps[provider.FriendlyName()] : DateTime.MinValue;
                    if (!provider.CompatibleWith(entity.Kind)) { continue; }
                    //if (!Forced && !provider.isStale(lastAccess)) { continue; }

                    // execute the payload
                    ProviderOutcome outcome = provider.Fetch(entity);

                    if (outcome == ProviderOutcome.Success)
                    {
                        requiresUpdate = true;
                        //entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
                    }
                    else if (outcome != ProviderOutcome.NoData) // no data is a warning, ignore it and move on
                    {
//                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Engines.Logging.LoggerEngineFactory.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
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
