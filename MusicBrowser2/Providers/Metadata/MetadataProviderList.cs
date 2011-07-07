using System;
using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Providers.Metadata
{
    class MetadataProviderList : IBackgroundTaskable
    {
        private static IEnumerable<IMetadataProvider> GetProviders()
        {
            List<IMetadataProvider> providerList = new List<IMetadataProvider>();

            providerList.Add(new TagSharpMetadataProvider());
            providerList.Add(new MediaInfoProvider());
            providerList.Add(new HTBackdropMetadataProvider());
            providerList.Add(new LastFMMetadataProvider());
            // add new providers here
            
            return providerList;
        }

        public static void ProcessEntity(IEntity entity)
        {
#if DEBUG
            Logging.LoggerFactory.Verbose("executing background metadata provider task for " + entity.Path, "start");
#endif
            foreach (IMetadataProvider provider in GetProviders())
            {
#if DEBUG
                Logging.LoggerFactory.Verbose(provider.GetType().ToString(), "start");
#endif
                try
                {
                    entity = provider.Fetch(entity);
                }
                catch (Exception e)
                {
#if DEBUG
                    Logging.LoggerFactory.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
#endif
                }
            }
            entity.CalculateValues();
            CacheEngineFactory.GetCacheEngine().Update(entity.CacheKey, EntityPersistance.Serialize(entity));
        }

        private readonly IEntity _entity;

        public MetadataProviderList(IEntity entity)
        {
            _entity = entity;
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
                ProcessEntity(_entity);
            }
            catch (Exception e)
            {
                Logging.LoggerFactory.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
            }

        }

        #endregion
    }
}
