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

            #endregion

            bool hasUpdated = false;

            Statistics.GetInstance().Hit(Name + ".hit");
            EntityFactory factory = new EntityFactory();

            #region album
            if (dto.DataType == DataTypes.Album)
            {
                DateTime albumDate = DateTime.MinValue;
                string ArtistName = new string(' ', 256);
                System.Drawing.Bitmap thumb = null;

                // data on <albums> <= tracks
                #region inherit

                IEnumerable<FileSystemItem> children = FileSystemProvider.GetFolderContents(dto.Path);
                foreach (FileSystemItem child in children)
                {
                    IEntity e = factory.GetItem(child.FullPath);
                    if (e.Kind == EntityKind.Song || e.Kind == EntityKind.Album)
                    {
                        if (e.ReleaseDate != null)
                        {
                            if (e.ReleaseDate > albumDate)
                            {
                                albumDate = e.ReleaseDate;
                            }
                        }
                        if (!string.IsNullOrEmpty(e.AlbumArtist))
                        {
                            if (ArtistName.Length > e.AlbumArtist.Length)
                            {
                                ArtistName = e.AlbumArtist;
                            }
                        }
                        if (thumb == null && !dto.hasThumbImage)
                        {
                            if (!String.IsNullOrEmpty(e.IconPath))
                            {
                                thumb = ImageProvider.Load(e.IconPath);
                            }
                        }
                    }
                }

                if (albumDate > DateTime.Parse("01-JAN-1000")) 
                {
                    if (dto.ReleaseDate != albumDate)
                    {
                        dto.ReleaseDate = albumDate;
                        hasUpdated = true;
                    }
                }
                ArtistName = ArtistName.Trim();
                if (!String.IsNullOrEmpty(ArtistName)) 
                {
                    if (dto.ArtistName != ArtistName)
                    {
                        dto.AlbumArtist = ArtistName.Trim();
                        hasUpdated = true;
                    }
                }
                if (thumb != null && !dto.hasThumbImage) 
                { 
                    dto.ThumbImage = thumb; 
                    dto.hasThumbImage = true;
                    hasUpdated = true;
                }

                #endregion

                // data on artists => <albums>
                #region decent

                IEntity parent = factory.GetItem(Directory.GetParent(dto.Path).FullName);
                if (parent.Kind == EntityKind.Artist)
                {
                    if (!String.IsNullOrEmpty(parent.BackgroundPath) && !dto.hasBackImage)
                    {
                        dto.BackImage = ImageProvider.Load(parent.BackgroundPath);
                        dto.hasBackImage = true;
                        hasUpdated = true;
                    }
                }

                #endregion
            }
            #endregion

            #region song

            if (dto.DataType == DataTypes.Song)
            {
                if (!dto.hasThumbImage && Util.Config.GetInstance().GetBooleanSetting("UseFolderImageForTracks"))
                {
                    IEntity parent = factory.GetItem(Directory.GetParent(dto.Path).FullName);
                    if (parent.Kind == EntityKind.Album && !String.IsNullOrEmpty(parent.IconPath))
                    {
                        dto.ThumbImage = ImageProvider.Load(parent.IconPath);
                        dto.hasThumbImage = true;
                        hasUpdated = true;
                    }
                }
            }

            #endregion

            if (!hasUpdated)
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new List<string> { "No updates made" };
            }

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "album") || (type.ToLower() == "song");
        }

        public bool isStale(DateTime lastAccess)
        {
            // always refesh
            return true;
        }
    }
}
