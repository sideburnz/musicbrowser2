using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.CacheEngine;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    // populates children and all descendants

    class FileSystemMetadataProvider : IDataProvider
    {
        private const string Name = "FileSystem";

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
            //Logging.Logger.Debug(Name + ": " + dto.Path);

            #region killer questions
            if (!Directory.Exists(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }

            #endregion

            int descendants = 0;
            int children = 0;
            int duration = 0;

            ICacheEngine cacheEngine = CacheEngineFactory.GetCacheEngine();

            IEnumerable<FileSystemItem> allPaths = FileSystemProvider.GetAllSubPaths(dto.Path);
            foreach (FileSystemItem item in allPaths)
            {
                if (Util.Helper.IsSong(item.FullPath))
                {
                    descendants++;

                    //TODO: can this be done smarter? - Tag#lite?
                    IEntity e = EntityPersistance.Deserialize(cacheEngine.Read(Util.Helper.GetCacheKey(item.FullPath)));
                    duration += e.Duration;
                }
            }
            dto.TrackCount = descendants;

            IEnumerable<FileSystemItem> childPaths = FileSystemProvider.GetFolderContents(dto.Path);
            foreach (FileSystemItem item in childPaths)
            {
                if (Util.Helper.IsSong(item.FullPath))
                {
                    children++;
                }
                else if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    children++;
                }
            }
            dto.Children = children;
            dto.Duration = duration;

            dto.Outcome = DataProviderOutcome.Success;
            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return ((type.ToLower() == "album") || (type.ToLower() == "artist") || (type.ToLower() == "genre"));
        }

        public bool isStale(DateTime lastAccess)
        {
            // refresh weekly
            return (lastAccess.AddDays(7) > DateTime.Now);
        }
    }
}
