//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
//using System.IO;
//using MusicBrowser.Engines.Cache;
//using MusicBrowser.Entities;
//using MusicBrowser.Interfaces;

//namespace MusicBrowser.Providers.Metadata
//{
//    class InheritanceMetadataProvider : IDataProvider
//    {
//        private const string Name = "InheritanceMatadataProvider";

//        private const int MinDaysBetweenHits = 1;
//        private const int MaxDaysBetweenHits = 5;
//        private const int RefreshPercentage = 50;

//        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

//        public DataProviderDTO Fetch(DataProviderDTO dto)
//        {
//#if DEBUG
//            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
//#endif
//            dto.Outcome = DataProviderOutcome.Success;

//            #region killer questions

//            #endregion

//            bool hasUpdated = false;

//            Statistics.Hit(Name + ".hit");
 
//            #region album
//            if (dto.DataType == DataTypes.Album)
//            {
//                DateTime albumDate = DateTime.MinValue;
//                string ArtistName = new string(' ', 256);
//                System.Drawing.Bitmap thumb = null;

//                // data on <albums> <= tracks
//                #region inherit

//                IEnumerable<FileSystemItem> children = FileSystemProvider.GetFolderContents(dto.Path);
//                foreach (FileSystemItem child in children)
//                {
//                    Entity e = EntityFactory.GetItem(child.FullPath);
//                    if (e == null) { continue; }

//                    if (e.Kind == EntityKind.Track || e.Kind == EntityKind.Album)
//                    {
//                        if (e.ReleaseDate != null)
//                        {
//                            if (e.ReleaseDate > albumDate)
//                            {
//                                albumDate = e.ReleaseDate;
//                            }
//                        }
//                        if (!string.IsNullOrEmpty(e.AlbumArtist))
//                        {
//                            if (ArtistName.Length > e.AlbumArtist.Length)
//                            {
//                                ArtistName = e.AlbumArtist;
//                            }
//                        }
//                        if (thumb == null && !dto.hasThumbImage)
//                        {
//                            if (!String.IsNullOrEmpty(e.IconPath))
//                            {
//                                thumb = ImageProvider.Load(e.IconPath);
//                            }
//                        }
//                    }
//                }

//                if (albumDate > DateTime.Parse("01-JAN-1000")) 
//                {
//                    if (dto.ReleaseDate != albumDate)
//                    {
//                        dto.ReleaseDate = albumDate;
//                        hasUpdated = true;
//                    }
//                }
//                ArtistName = ArtistName.Trim();
//                if (!String.IsNullOrEmpty(ArtistName)) 
//                {
//                    if (dto.ArtistName != ArtistName)
//                    {
//                        dto.AlbumArtist = ArtistName.Trim();
//                        hasUpdated = true;
//                    }
//                }
//                if (thumb != null && !dto.hasThumbImage) 
//                { 
//                    dto.ThumbImage = thumb; 
//                    dto.hasThumbImage = true;
//                    hasUpdated = true;
//                }

//                #endregion

//                // data on artists => <albums>
//                #region decent

//                Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
//                if (parent.Kind == EntityKind.Artist)
//                {
//                    if (!(parent.BackgroundPaths.FirstOrDefault() == null) && !dto.hasBackImage)
//                    {
//                        dto.BackImages.Add(ImageProvider.Load(parent.BackgroundPaths[Rnd.Next(parent.BackgroundPaths.Count)]));
//                        dto.hasBackImage = true;
//                        hasUpdated = true;
//                    }
//                }

//                #endregion
//            }
//            #endregion

//            #region track

//            if (dto.DataType == DataTypes.Track)
//            {
//                if (!dto.hasThumbImage && Util.Config.GetInstance().GetBooleanSetting("UseFolderImageForTracks"))
//                {
//                    Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
//                    if (parent.Kind == EntityKind.Album && !String.IsNullOrEmpty(parent.IconPath))
//                    {
//                        dto.ThumbImage = ImageProvider.Load(parent.IconPath);
//                        dto.hasThumbImage = true;
//                        hasUpdated = true;
//                    }
//                }
//            }

//            #endregion

//            if (!hasUpdated)
//            {
//                dto.Outcome = DataProviderOutcome.NoData;
//                dto.Errors = new List<string> { "No updates made" };
//            }

//            return dto;
//        }

//        public string FriendlyName()
//        {
//            return Name;
//        }

//        public bool CompatibleWith(string type)
//        {
//            return (type.ToLower() == "album") || (type.ToLower() == "track");
//        }

//        /// <summary>
//        /// refresh requests between the min and max refresh period have 10% chance of refreshing
//        /// </summary>
//        private static bool RandomlyRefreshData(DateTime stamp)
//        {
//            // if it's never refreshed, refresh it
//            if (stamp < DateTime.Parse("01-JAN-1000")) { return true; }

//            // if it's less then the min, don't refresh if it's older than the max then do refresh
//            int dataAge = (DateTime.Today.Subtract(stamp)).Days;
//            if (dataAge <= MinDaysBetweenHits) { return false; }
//            if (dataAge >= MaxDaysBetweenHits) { return true; }

//            // otherwise refresh randomly (95% don't refresh each run)
//            return (Rnd.Next(100) >= RefreshPercentage);
//        }

//        public bool isStale(DateTime lastAccess)
//        {
//            return RandomlyRefreshData(lastAccess);
//        }

//        public ProviderType Type
//        {
//            get { return ProviderType.Core; }
//        }
//    }
//}
