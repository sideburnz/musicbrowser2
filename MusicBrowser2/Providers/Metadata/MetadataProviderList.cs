using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Background;
using MusicBrowser.Entities.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class MetadataProviderList : IBackgroundTaskable
    {
        private static List<IMetadataProvider> GetProviders()
        {
            List<IMetadataProvider> providerList = new List<IMetadataProvider>();

            providerList.Add(new TagSharpMetadataProvider());
            providerList.Add(new MediaInfoProvider());
            providerList.Add(new HTBackdropMetadataProvider());
            providerList.Add(new LastFMMetadataProvider());
            // add new providers here
            
            return providerList;
        }

        public static void ProcessEntity(IEntity entity, IEntityCache cache)
        {
#if DEBUG
            Logging.Logger.Verbose("executing background metadata provider task for " + entity.Path, "start");
#endif
            foreach (IMetadataProvider provider in GetProviders())
            {
#if DEBUG
                Logging.Logger.Verbose(provider.GetType().ToString(), "start");
#endif
                try
                {
                    entity = provider.Fetch(entity);
                }
                catch (Exception e)
                {
#if DEBUG
                    Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
#endif
                    }
            }
            entity.CalculateValues();
            cache.Update(entity.CacheKey, entity);
        }

        private IEntity _entity;
        private IEntityCache _cache;

        public MetadataProviderList(IEntity entity, IEntityCache cache)
        {
            _entity = entity;
            _cache = cache;
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
                ProcessEntity(_entity, _cache);
            }
            catch (Exception e)
            {
                Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
            }

        }

        #endregion
    }
}
