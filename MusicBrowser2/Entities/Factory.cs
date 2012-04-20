using System;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Metadata.Lite;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    public static class Factory
    {
        private static readonly ICacheEngine CacheEngine = CacheEngineFactory.GetEngine();
        private static readonly InMemoryCache MemCache = InMemoryCache.GetInstance();

        public static baseEntity GetItem(string item)
        {
            return GetItem(FileSystemProvider.GetItemDetails(item));
        }

        public static baseEntity GetItem(FileSystemItem item)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ")", "start");
#endif

            // don't waste time trying to determine a known not entity
            if (item.Name.ToLower() == "metadata") { return null; }
            if (Helper.GetKnownType(item) == Helper.KnownType.Other) { return null; }

            string key = Helper.GetCacheKey(item.FullPath);

            #region InMemoryCache
            // get from the Mem cache if it's cached there, this is the fastest cache
            baseEntity entity = MemCache.Fetch(key);
            if (entity != null)
            {
                return entity;
            }
            #endregion

            #region persistent cache
            // get the value from persistent cache
            entity = CacheEngine.Fetch(key);
            if (entity != null && entity.TimeStamp > item.LastUpdated)
            {
                MemCache.Update(entity);
                return entity;
            }
            Telemetry.Hit("factory.missorexpired");

            #endregion

            LoggerEngineFactory.Debug("Factory", "Manufacturing " + item.FullPath);

            Telemetry.Hit("factory.hit");

            EntityResolver.EntityKind? kind = EntityResolver.Resolve(item);
            if (kind == null) { return null; }

            switch (kind)
            {
                case EntityResolver.EntityKind.Album:
                    entity = Factorize<Album>(item); break;
                case EntityResolver.EntityKind.Artist:
                    entity = Factorize<Artist>(item); break;
                case EntityResolver.EntityKind.Folder:
                    entity = Factorize<Folder>(item); break;
                case EntityResolver.EntityKind.Genre:
                    entity = Factorize<Genre>(item); break;
                case EntityResolver.EntityKind.Season:
                    entity = Factorize<Season>(item); break;
                case EntityResolver.EntityKind.Show:
                    entity = Factorize<Show>(item); break;
                case EntityResolver.EntityKind.Playlist:
                    entity = Factorize<Playlist>(item); break;
                case EntityResolver.EntityKind.Track:
                    entity = Factorize<Track>(item);
                    MediaInfoProvider.FetchLite(entity);
                    break;
                case EntityResolver.EntityKind.Episode:
                    entity = Factorize<Episode>(item);
                    VideoFilenameMetadataProvider.FetchLite(entity);
                    MediaInfoProvider.FetchLite(entity);
                    break;
                case EntityResolver.EntityKind.Movie:
                    entity = Factorize<Movie>(item);
                    MediaInfoProvider.FetchLite(entity);
                    break;
            }

            entity.UpdateCache();
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("Factory.getItem(" + item.FullPath + ") = " + entity.Kind + " - first principles", "finish");
#endif
            return entity;
        }

        // use some magic to create the entity and set some initial values
        private static T Factorize<T>(FileSystemItem item) where T : baseEntity, new()
        {
            T e = new T();
            if ((item.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                e.Title = Path.GetFileNameWithoutExtension(item.Name);
            }
            else
            {
                e.Title = item.Name;
            }
            e.Path = item.FullPath;
            e.LastUpdated = item.LastUpdated;
            e.TimeStamp = DateTime.Now;
            if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                e.ThumbPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Thumb);
                e.BannerPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Banner);
                e.LogoPath = ImageProvider.LocateFanArt(item.FullPath, ImageType.Logo);
                e.BackgroundPaths = ImageProvider.LocateBackdropList(item.FullPath);
            }
            return e;
        }





        // works out where the metadata file is (if there is one)
        //private static string MetadataPath(string item)
        //{
        //    string itemName = Path.GetFileNameWithoutExtension(item);
        //    string metadataPath = Directory.GetParent(item).FullName;
        //    FileSystemItem metadataFile;

        //    string metadataLocal = metadataPath + "\\" + itemName + "\\metadata.xml";
        //    metadataFile = FileSystemProvider.GetItemDetails(metadataLocal);
        //    if (!String.IsNullOrEmpty(metadataFile.Name))
        //    {
        //        return metadataFile.FullPath;
        //    }
        //    string metadataInParent = metadataPath + "\\metadata\\" + itemName + ".xml";
        //    metadataFile = FileSystemProvider.GetItemDetails(metadataInParent);
        //    // this either returns detail or an empty struct which would indicate not found
        //    if (!String.IsNullOrEmpty(metadataFile.Name))
        //    {
        //        return metadataFile.FullPath;
        //    }
        //    return String.Empty;
        //}
    }
}