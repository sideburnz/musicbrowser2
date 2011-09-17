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
        private static object obj = new object();
        private static IList<IDataProvider> _providers = null;

        public static IEnumerable<IDataProvider> GetProviders()
        {
            if (_providers == null)
            {
                lock (obj)
                {
                    if (_providers == null)
                    {
                        _providers = new List<IDataProvider>();

                        _providers.Add(new TagSharpMetadataProvider());
                        _providers.Add(new MediaInfoProvider());
                        _providers.Add(new InheritanceProvider());
                        if (Util.Config.GetInstance().GetBooleanSetting("UseInternetProviders"))
                        {
                            _providers.Add(new HTBackdropMetadataProvider());
                            _providers.Add(new LastFMMetadataProvider());
                        }
                        _providers.Add(new FileSystemMetadataProvider());
                        _providers.Add(new IconProvider());
                    }
                }
            }
            return _providers;
        }

        public static void ProcessEntity(Entity entity, bool Forced)
        {
#if DEBUG
            Logging.Logger.Verbose("ProcessEntity(" + entity.Path + ", <providers>, " + Forced + ")", "start");
#endif

            bool requiresUpdate = false;

            foreach (IDataProvider provider in GetProviders())
            {
                try
                {
                    DateTime lastAccess = entity.ProviderTimeStamps.ContainsKey(provider.FriendlyName()) ? entity.ProviderTimeStamps[provider.FriendlyName()] : DateTime.MinValue;
                    if (!provider.CompatibleWith(entity.KindName)) { continue; }
                    if (!Forced && !provider.isStale(lastAccess)) { continue; }
                    DataProviderDTO dto = PopulateDTO(entity);
                    dto = provider.Fetch(dto);
                    if (dto.Outcome == DataProviderOutcome.Success)
                    {
                        entity = PopulateEntity(entity, dto);
                        requiresUpdate = true;
                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
                    }
                    else if (dto.Outcome != DataProviderOutcome.NoData) // no data is a warning, ignore it and move on
                    {
                        Logging.Logger.Debug(dto.Outcome.ToString() + " " + dto.Errors[0]);
                        entity.ProviderTimeStamps[provider.FriendlyName()] = DateTime.Now;
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
                NearLineCache.GetInstance().Update(entity);
                CacheEngineFactory.GetCacheEngine().Update(entity.CacheKey, EntityPersistance.Serialize(entity));
            }
        }

        private static DataProviderDTO PopulateDTO(Entity entity)
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
            dto.Genre = entity.Genre;
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

            dto.TrackCount = entity.TrackCount;
            dto.Title = entity.Title;
            dto.Label = entity.Label;
            dto.AlbumCount = entity.AlbumCount;
            dto.ArtistCount = entity.ArtistCount;

            dto.hasThumbImage = !String.IsNullOrEmpty(entity.IconPath);
            dto.hasBackImage = !String.IsNullOrEmpty(entity.BackgroundPath);

            dto.ProviderTimeStamps = entity.ProviderTimeStamps;

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
                        dto.AlbumArtist = entity.Title;
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
        
        private static Entity PopulateEntity(Entity entity, DataProviderDTO dto)
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
            if (dto.Genre != null) { entity.Genre = dto.Genre; }
            if (!String.IsNullOrEmpty(dto.Label)) { entity.Label = dto.Label; }
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
            } 
            if (!String.IsNullOrEmpty(dto.Resolution)) { entity.Resolution = dto.Resolution; }
            if (!String.IsNullOrEmpty(dto.SampleRate)) { entity.SampleRate = dto.SampleRate; }
            if (!String.IsNullOrEmpty(dto.Summary)) { entity.Summary = dto.Summary; }
            if (!String.IsNullOrEmpty(dto.Title)) { entity.Title = dto.Title; }
            if (dto.TotalPlays > 0) { entity.TotalPlays = dto.TotalPlays; }
            if (dto.TrackNumber > 0) { entity.TrackNumber = dto.TrackNumber; }
            if (dto.AlbumCount > 0) { entity.AlbumCount = dto.AlbumCount; }
            if (dto.ArtistCount > 0) { entity.ArtistCount = dto.ArtistCount; }
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

            switch (dto.DataType)
            {
                case DataTypes.Album:
                    {
                        if (!String.IsNullOrEmpty(dto.AlbumName)) 
                        {
                            entity.Title = dto.AlbumName;  
                        }
                        break;
                    }
                case DataTypes.Artist:
                    {
                        if (!String.IsNullOrEmpty(dto.ArtistName))
                        {
                            entity.Title = dto.ArtistName;
                        }
                        break;
                    }
                case DataTypes.Song:
                    {
                        if (!String.IsNullOrEmpty(dto.TrackName))
                        {
                            entity.Title = dto.TrackName;
                        }
                        break;
                    }
            }
            
            return entity;
        }

        private readonly Entity _entity;
        private readonly bool _forced;

        public MetadataProviderList(Entity entity)
        {
            _entity = entity;
            _forced = false;
        }

        public MetadataProviderList(Entity entity, bool forced)
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
                Logging.Logger.Error(new Exception(string.Format("MetadataProviderList failed for {0}\r", _entity.Path), e));
            }

        }

        #endregion
    }
}
