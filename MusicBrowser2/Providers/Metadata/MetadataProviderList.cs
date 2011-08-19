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
            providerList.Add(new FileSystemMetadataProvider());
            providerList.Add(new MediaInfoProvider());
            //providerList.Add(new HTBackdropMetadataProvider());
            //providerList.Add(new LastFMMetadataProvider());
            // add new providers here
            
            return providerList;
        }

        public static void ProcessEntity(IEntity entity)
        {
            bool requiresUpdate = false;

            foreach (IDataProvider provider in GetProviders())
            {
                try
                {
                    DateTime lastAccess = entity.ProviderTimeStamps.ContainsKey(provider.FriendlyName()) ? entity.ProviderTimeStamps[provider.FriendlyName()] : DateTime.MinValue;

                    if (!provider.CompatibleWith(entity.KindName)) { continue; }
                    if (!provider.isStale(lastAccess)) { continue; }

                    DataProviderDTO dto = PopulateDTO(entity);
                    dto = provider.Fetch(dto);
                    if (dto.Outcome == DataProviderOutcome.Success)
                    {
                        entity = PopulateEntity(entity, dto);
                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
                        requiresUpdate = true;
                    }
                    else
                    {
                        Logging.Logger.Debug(dto.Outcome.ToString() + " " + dto.Errors[0]);
                    }

                }
                catch (Exception e)
                {
#if DEBUG
                    Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed whilst running {0} for {1}\r", provider.GetType().ToString(), entity.Path), e));
#endif
                }
            }
            if (requiresUpdate)
            {
                entity.UpdateValues();
                CacheEngineFactory.GetCacheEngine().Update(entity.CacheKey, EntityPersistance.Serialize(entity));
            }
        }

        private static DataProviderDTO PopulateDTO(IEntity entity)
        {
            DataProviderDTO dto = new DataProviderDTO();

            dto.AlbumArtist = entity.AlbumArtist;
            dto.AlbumName = entity.AlbumName;
            dto.ArtistName = entity.ArtistName;
            dto.Channels = entity.Channels;
            dto.Codec = entity.Codec;
            dto.DiscId = entity.DiscId;
            dto.DiscNumber = entity.DiscNumber;
            dto.Duration = entity.Duration;
            dto.Favorite = entity.Favorite;
            dto.Genres = entity.Genres;
            dto.Listeners = entity.Listeners;
            dto.Lyrics = entity.Lyrics;
            dto.MusicBrainzId = entity.MusicBrainzID;
            dto.Path = entity.Path;
            dto.Performers = entity.Performers;
            dto.PlayCount = entity.PlayCount;
            dto.Rating = entity.Rating;
            dto.ReleaseDate = entity.ReleaseDate;
            dto.Resolution = entity.Resolution;
            dto.SampleRate = entity.SampleRate;
            dto.Summary = entity.Summary;
            dto.TotalPlays = entity.TotalPlays;
            dto.TrackNumber = entity.TrackNumber;

            dto.Children = entity.Children;
            dto.TrackCount = entity.TrackCount;
            dto.Title = entity.Title;

            switch (entity.Kind)
            {
                    case EntityKind.Album:
                    {
                        dto.DataType = DataTypes.Album;
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
                        dto.TrackName = entity.Title;
                        break;
                    }
                    case EntityKind.Genre:
                    {
                        dto.DataType = DataTypes.Genre;
                        break;
                    }
            }

            return dto;
        }
        
        private static IEntity PopulateEntity(IEntity entity, DataProviderDTO dto)
        {

            if (!String.IsNullOrEmpty(dto.AlbumArtist)) { entity.AlbumArtist = dto.AlbumArtist; }
            if (!String.IsNullOrEmpty(dto.AlbumName)) { entity.AlbumName = dto.AlbumName; }
            if (!String.IsNullOrEmpty(dto.ArtistName)) { entity.ArtistName = dto.ArtistName; }
            if (!String.IsNullOrEmpty(dto.Channels)) { entity.Channels = dto.Channels; }
            if (!String.IsNullOrEmpty(dto.Codec)) { entity.Codec = dto.Codec; }
            if (!String.IsNullOrEmpty(dto.DiscId)) { entity.DiscId = dto.DiscId; }
            if (dto.DiscNumber > 0) { entity.DiscNumber = dto.DiscNumber; }
            if (dto.Duration > 0) { entity.Duration = dto.Duration; }
            entity.Favorite = dto.Favorite;
            if (dto.Genres != null) { entity.Genres = dto.Genres; }
            if (dto.Listeners > 0) { entity.Listeners = dto.Listeners; }
            if (!String.IsNullOrEmpty(dto.Lyrics)) { entity.Lyrics = dto.Lyrics; }
            if (!String.IsNullOrEmpty(dto.MusicBrainzId)) { entity.MusicBrainzID = dto.MusicBrainzId; }
            if (!String.IsNullOrEmpty(dto.Path)) { entity.Path = dto.Path; }
            if (dto.Performers != null) { entity.Performers = dto.Performers; }
            if (dto.PlayCount > 0) { entity.PlayCount = dto.PlayCount; }
            if (dto.Rating > 0) { entity.Rating = dto.Rating; }
            if (entity.ReleaseDate < DateTime.Parse("01-JAN-1000")) 
            { 
                entity.ReleaseDate = dto.ReleaseDate;
                if (entity.Parent.ReleaseDate < DateTime.Parse("01-JAN-1000"))
                {
                    entity.Parent.ReleaseDate = dto.ReleaseDate;
                }
            } 
            if (!String.IsNullOrEmpty(dto.Resolution)) { entity.Resolution = dto.Resolution; }
            if (!String.IsNullOrEmpty(dto.SampleRate)) { entity.SampleRate = dto.SampleRate; }
            if (!String.IsNullOrEmpty(dto.Summary)) { entity.Summary = dto.Summary; }
            if (!String.IsNullOrEmpty(dto.Title)) { entity.Title = dto.Title; }
            if (dto.TotalPlays > 0) { entity.TotalPlays = dto.TotalPlays; }
            if (dto.TrackNumber > 0) { entity.TrackNumber = dto.TrackNumber; }
            if (dto.Children > 0) { entity.Children = dto.Children; }
            if (dto.TrackCount > 0) { entity.TrackCount = dto.TrackCount; }

            if (dto.ThumbImage != null)
            {
                string iconPath = Util.Helper.ImageCacheFullName(entity.CacheKey, "Thumbs");
                ImageProvider.Save(ImageProvider.Resize(dto.ThumbImage, ImageType.Thumb), iconPath);
                entity.IconPath = iconPath;
            }

            if (dto.BackImage != null)
            {
                string backgroundPath = Util.Helper.ImageCacheFullName(entity.CacheKey, "Backgrounds");
                ImageProvider.Save(ImageProvider.Resize(dto.BackImage, ImageType.Backdrop), backgroundPath);
                entity.BackgroundPath = backgroundPath;
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
