using System;
using System.Collections.Generic;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Providers.Background;

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
                    DataProviderDTO dto = PopulateDTO(entity);
                    dto = provider.Fetch(dto);
                    if (dto.Outcome == DataProviderOutcome.Success)
                    {
                        entity = PopulateEntity(entity, dto);
                    }
                    else
                    {
                        Logging.Logger.Debug(dto.Outcome.ToString());
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

        private static DataProviderDTO PopulateDTO(IEntity entity)
        {
            DataProviderDTO dto = new DataProviderDTO();

            dto.Path = entity.Path;
            dto.MusicBrainzId = entity.MusicBrainzID;

            switch (entity.Kind)
            {
                    case EntityKind.Album:
                    {
                        dto.DataType = DataTypes.Album;
                        dto.AlbumArtist = entity.AlbumArtist;
                        dto.AlbumName = entity.Title;

                        break;
                    }
                    case EntityKind.Artist:
                    {
                        dto.DataType = DataTypes.Artist;
                        dto.ArtistName = entity.Title;

                        break;
                    }
                    case EntityKind.Disc:
                    {
                        dto.DataType = DataTypes.Disc;
                        dto.DiscId = entity.DiscId;
                        break;
                    }
                    case EntityKind.Playlist:
                    {
                        dto.DataType = DataTypes.Playlist;
                        break;
                    }
                    case EntityKind.Song:
                    {
                        dto.DataType = DataTypes.Song;
                        dto.AlbumArtist = entity.AlbumArtist;
                        dto.AlbumName = entity.AlbumName;
                        dto.ArtistName = entity.ArtistName;
                        dto.TrackName = entity.Title;

                        break;
                    }
                default:
                    {
                        break;
                    }
            
            }

            return dto;
        }
        
        private static IEntity PopulateEntity(IEntity entity, DataProviderDTO dto)
        {

            entity.Duration = dto.Duration;
            entity.Favorite = dto.Favorite;
            entity.Genres = dto.Genres;
            entity.Listeners = dto.Listeners;
            entity.Performers = dto.Performers;
            entity.PlayCount = dto.PlayCount;
            entity.TotalPlays = dto.TotalPlays;
            entity.Rating = dto.Rating;

            if (!String.IsNullOrEmpty(dto.AlbumArtist)) { entity.AlbumArtist = dto.AlbumArtist; }
            if (!String.IsNullOrEmpty(dto.AlbumName)) { entity.AlbumName = dto.AlbumName; }
            if (!String.IsNullOrEmpty(dto.ArtistName)) { entity.ArtistName = dto.ArtistName; }

            if (dto.ReleaseDate != DateTime.MinValue) { entity.ReleaseDate = dto.ReleaseDate; }
            if (!String.IsNullOrEmpty(dto.Summary)) { entity.Summary = dto.Summary; }
            if (!String.IsNullOrEmpty(dto.MusicBrainzId)) { entity.MusicBrainzID = dto.MusicBrainzId; }

//            entity.IconPath = ImageProvider.Save(dto.ThumbImage, "");
//            entity.BackgroundPath = ImageProvider.Save(dto.BackImage, "");
            
            switch (dto.DataType)
            {
                    case DataTypes.Album:
                    {
                        if (!String.IsNullOrEmpty(dto.AlbumName)) { entity.Title = dto.AlbumName; }
                        break;
                    }
                    case DataTypes.Artist:
                    {
                        if (!String.IsNullOrEmpty(dto.ArtistName)) { entity.Title = dto.ArtistName; }
                        break;
                    }
                    case DataTypes.Playlist:
                    {
                        if (!String.IsNullOrEmpty(dto.TrackName)) { entity.Title = dto.TrackName; }
                        break;
                    }
                    case DataTypes.Song:
                    {
                        if (!String.IsNullOrEmpty(dto.TrackName)) { entity.Title = dto.TrackName; }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return entity;
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
