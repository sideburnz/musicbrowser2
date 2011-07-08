using System;
using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class MetadataProviderList : IBackgroundTaskable
    {
        private static IEnumerable<IDataProvider> GetProviders()
        {
            List<IDataProvider> providerList = new List<IDataProvider>();

            providerList.Add(new TagSharpMetadataProvider());
            //providerList.Add(new MediaInfoProvider());
            //providerList.Add(new HTBackdropMetadataProvider());
            //providerList.Add(new LastFMMetadataProvider());
            // add new providers here
            
            return providerList;
        }

        public static void ProcessEntity(IEntity entity)
        {
            foreach (IDataProvider provider in GetProviders())
            {
                try
                {
                    DataProviderDTO dto = provider.GetDTO();
                    dto.Parameters["path"] = entity.Path;
                    dto = provider.Fetch(dto);
                    if (dto.Outcome == DataProviderOutcome.Success)
                    {
                        entity.Title = dto.Parameters["title"];
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
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
                Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
            }

        }

        #endregion
    }
}
