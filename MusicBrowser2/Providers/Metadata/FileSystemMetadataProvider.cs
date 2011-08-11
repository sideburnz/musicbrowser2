using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Providers.Metadata
{
    // populates children and all descendants

    class FileSystemMetadataProvider : IDataProvider
    {
        private const string Name = "FileSystem";

        public DataProviderDTO Fetch(DataProviderDTO dto, DateTime lastAccess)
        {
            Logging.Logger.Debug(Name + ": " + dto.Path);

            #region killer questions
            if (Util.Helper.IsSong(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }

            if (Util.Helper.IsPlaylist(dto.Path))
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a folder: " + dto.Path };
                return dto;
            }

            FileSystemItem entity = FileSystemProvider.GetItemDetails(dto.Path);
            if ((lastAccess.AddDays(7) > DateTime.Now) && (lastAccess > entity.LastUpdated))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new List<string> { "Not Refreshing Data: " + dto.Path };
                return dto;
            }
            #endregion

            int descendants = 0;
            int children = 0;
            //TODO: int Duration = 0;

            IEnumerable<FileSystemItem> allPaths = FileSystemProvider.GetAllSubPaths(dto.Path);
            foreach (FileSystemItem item in allPaths)
            {
                if (Util.Helper.IsSong(item.FullPath))
                {
                    descendants++;
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
    }
}
