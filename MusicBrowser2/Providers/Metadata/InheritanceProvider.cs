using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    class InheritanceProvider : IDataProvider
    {
        private const string Name = "InheritanceProvider";
        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

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
                    Entity e = EntityFactory.GetItem(child.FullPath);
                    if (e == null) { continue; }

                    if (e.Kind == EntityKind.Track || e.Kind == EntityKind.Album)
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

                Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
                if (parent.Kind == EntityKind.Artist)
                {
                    if (!(parent.BackgroundPaths.FirstOrDefault() == null) && !dto.hasBackImage)
                    {
                        dto.BackImages.Add(ImageProvider.Load(parent.BackgroundPaths[Rnd.Next(parent.BackgroundPaths.Count)]));
                        dto.hasBackImage = true;
                        hasUpdated = true;
                    }
                }

                #endregion
            }
            #endregion

            #region track

            if (dto.DataType == DataTypes.Track)
            {
                if (!dto.hasThumbImage && Util.Config.GetInstance().GetBooleanSetting("UseFolderImageForTracks"))
                {
                    Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
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
            return (type.ToLower() == "album") || (type.ToLower() == "track");
        }

        public bool isStale(DateTime lastAccess)
        {
            // always refesh
            return true;
        }


        public ProviderSpeed Speed
        {
            get { return ProviderSpeed.Fast; }
        }
    }
}
