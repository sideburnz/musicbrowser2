using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class InheritanceProvider : IDataProvider
    {
        private const string Name = "InheritanceProvider";

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            if (!Directory.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }

            #endregion

            Statistics.GetInstance().Hit(Name + ".hit");

            DateTime albumDate = DateTime.MinValue;
            string ArtistName = new string(' ', 256);
            System.Drawing.Bitmap thumb = null; 

            ICacheEngine cacheEngine = CacheEngineFactory.GetCacheEngine();

            IEnumerable<FileSystemItem> children = FileSystemProvider.GetFolderContents(dto.Path);
            foreach (FileSystemItem child in children)
            {
                IEntity e = EntityPersistance.Deserialize(cacheEngine.Read(Util.Helper.GetCacheKey(child.FullPath)));
                
                if (e.ReleaseDate > albumDate) { albumDate = e.ReleaseDate; }
                if (ArtistName.Length > e.AlbumArtist.Length) { ArtistName = e.AlbumArtist; }
                if (thumb == null) { if (!String.IsNullOrEmpty(e.IconPath)) { thumb = ImageProvider.Load(e.IconPath); } }
            }

            if (albumDate > DateTime.Parse("01-JAN-1000")) { dto.ReleaseDate = albumDate; }
            if (!String.IsNullOrEmpty(ArtistName)) { dto.AlbumArtist = ArtistName; }
            if (thumb != null) { dto.ThumbImage = thumb; }

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "album");
        }

        public bool isStale(DateTime lastAccess)
        {
            // refresh weekly
            return (lastAccess.AddDays(7) < DateTime.Now);
        }
    }
}
