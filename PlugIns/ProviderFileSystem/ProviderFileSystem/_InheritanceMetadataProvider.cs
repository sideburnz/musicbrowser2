//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
//using System.IO;
//using MusicBrowser.Engines.Cache;
//using MusicBrowser.Entities;
//using MusicBrowser.Interfaces;
//using MusicBrowser.Util;
//using MusicBrowser.Providers;

//namespace MusicBrowser.Engines.Metadata
//{
//    class InheritanceMetadataProvider : baseMetadataProvider
//    {
//        public InheritanceMetadataProvider()
//        {
//            Name = "InheritanceMatadataProvider";
//            MinDaysBetweenHits = 1;
//            MaxDaysBetweenHits = 5;
//            RefreshPercentage = 50;
//        }

//        public override bool CompatibleWith(baseEntity dto)
//        {
//            return dto.InheritsFrom<Album>();
//        }

//        public override bool AskKillerQuestions(baseEntity dto)
//        {
//            if (!CompatibleWith(dto)) { return false; }
//            return true;
//        }

//        public override ProviderOutcome DoWork(baseEntity dto)
//        {


//            //                #endregion

//            //                // data on artists => <albums>
//            //                #region decent

//            //                Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
//            //                if (parent.Kind == EntityKind.Artist)
//            //                {
//            //                    if (!(parent.BackgroundPaths.FirstOrDefault() == null) && !dto.hasBackImage)
//            //                    {
//            //                        dto.BackImages.Add(ImageProvider.Load(parent.BackgroundPaths[Rnd.Next(parent.BackgroundPaths.Count)]));
//            //                        dto.hasBackImage = true;
//            //                        hasUpdated = true;
//            //                    }
//            //                }

//            //                #endregion
//            //            }
//            //            #endregion

//            //            #region track

//            //            if (dto.DataType == DataTypes.Track)
//            //            {
//            //                if (!dto.hasThumbImage && Util.Config.GetInstance().GetBooleanSetting("UseFolderImageForTracks"))
//            //                {
//            //                    Entity parent = EntityFactory.GetItem(Directory.GetParent(dto.Path).FullName);
//            //                    if (parent.Kind == EntityKind.Album && !String.IsNullOrEmpty(parent.IconPath))
//            //                    {
//            //                        dto.ThumbImage = ImageProvider.Load(parent.IconPath);
//            //                        dto.hasThumbImage = true;
//            //                        hasUpdated = true;
//            //                    }
//            //                }
//            //            }

//            //            #endregion
//            return ProviderOutcome.InvalidInput;
//        }

//        private bool DoWorkAlbum(Album dto)
//        {
//            DateTime albumDate = DateTime.MinValue;
//            string ArtistName = new string(' ', 256);

//            // data on <albums> <= tracks

//            IEnumerable<FileSystemItem> children = FileSystemProvider.GetFolderContents(dto.Path).FilterDVDFiles();
//            foreach (FileSystemItem child in children)
//            {
//                // quick check if its a track
//                if (Helper.getKnownType(child) != Helper.knownType.Track) { continue; }

//                baseEntity e = EntityFactory.GetItem(child.FullPath);
//                if (e == null) { continue; }

//                if (e.InheritsFrom<Track>())
//                {
//                    if (e.ReleaseDate != null)
//                    {
//                        if (e.ReleaseDate > albumDate)
//                        {
//                            albumDate = e.ReleaseDate;
//                        }
//                    }
//                    if (!string.IsNullOrEmpty(e.AlbumArtist))
//                    {
//                        if (ArtistName.Length > e.AlbumArtist.Length)
//                        {
//                            ArtistName = e.AlbumArtist;
//                        }
//                    }
//                    if (thumb == null && !dto.hasThumbImage)
//                    {
//                        if (!String.IsNullOrEmpty(e.IconPath))
//                        {
//                            thumb = ImageProvider.Load(e.IconPath);
//                        }
//                    }
//                }
//            }

//            if (albumDate > DateTime.Parse("01-JAN-1000"))
//            {
//                if (dto.ReleaseDate != albumDate)
//                {
//                    dto.ReleaseDate = albumDate;
//                    hasUpdated = true;
//                }
//            }
//            ArtistName = ArtistName.Trim();
//            if (!String.IsNullOrEmpty(ArtistName))
//            {
//                if (dto.ArtistName != ArtistName)
//                {
//                    dto.AlbumArtist = ArtistName.Trim();
//                    hasUpdated = true;
//                }
//            }
//            if (thumb != null && !dto.hasThumbImage)
//            {
//                dto.ThumbImage = thumb;
//                dto.hasThumbImage = true;
//                hasUpdated = true;
//            }
//            return true;
//        }
//    }
//}
