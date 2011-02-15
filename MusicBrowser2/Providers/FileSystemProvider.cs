using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MusicBrowser.Providers.FolderItems.InterOp;

namespace MusicBrowser.Providers
{
    public struct FileSystemItem
    {
        public readonly string FullPath;
        public readonly string Name;
        public readonly FileAttributes Attributes;
        public readonly DateTime LastUpdated;
        public readonly DateTime Created;

        internal FileSystemItem(string fullPath, string name, FileAttributes attributes, DateTime lastUpdated, DateTime created)
        {
            FullPath = fullPath;
            Name = name;
            Attributes = attributes;
            LastUpdated = lastUpdated;
            Created = created;
        }
    }

    public static class FileSystemProvider
    {
        public static FileSystemItem getItemDetails(string path)
        {
            IntPtr invalidHandleValue = new IntPtr(-1);
            FileSystem.Win32FindDataw findData;
            IntPtr findHandle = invalidHandleValue;
            FileSystemItem info = new FileSystemItem();

            try
            {
                findHandle = FileSystem.FindFirstFileW(path, out findData);
                if (findHandle != invalidHandleValue)
                {
                    // ignore hidden files and folders
                    if ((findData.dwFileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        FileSystemItem fsi = new FileSystemItem(path, 
                                                                findData.cFileName,
                                                                findData.dwFileAttributes,
                                                                ToDateTime(findData.ftLastWriteTime),
                                                                ToDateTime(findData.ftCreationTime));
                        info = fsi;
                    }
                }
            }
            finally
            {
                if (findHandle != invalidHandleValue)
                {
                    FileSystem.FindClose(findHandle);
                }
            }
            return info;
        }

        public static IEnumerable<FileSystemItem> GetFolderContents(string directory)
        {
            IntPtr invalidHandleValue = new IntPtr(-1);
            FileSystem.Win32FindDataw findData;
            IntPtr findHandle = invalidHandleValue;

            List<FileSystemItem> info = new List<FileSystemItem>();
            try
            {
                findHandle = FileSystem.FindFirstFileW(directory + @"\*", out findData);
                if (findHandle != invalidHandleValue)
                {
                    do
                    {
                        if (findData.cFileName.Equals(".") || findData.cFileName.Equals("..")) continue;
                        string fullpath = directory + (directory.EndsWith(@"\") ? "" : @"\") + findData.cFileName;
                        // ignore hidden files and folders
                        if ((findData.dwFileAttributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            FileSystemItem fsi = new FileSystemItem(fullpath.ToLower(), 
                                                                    findData.cFileName,
                                                                    findData.dwFileAttributes,
                                                                    ToDateTime(findData.ftLastWriteTime),
                                                                    ToDateTime(findData.ftCreationTime));
                            info.Add(fsi);
                        }
                    }
                    while (FileSystem.FindNextFile(findHandle, out findData));
                }
            }
            finally
            {
                if (findHandle != invalidHandleValue)
                {
                    FileSystem.FindClose(findHandle);
                }
            }
            return info;
        }

        public static IEnumerable<FileSystemItem> getAllSubPaths(string dir)
        {
            List<FileSystemItem> temp = new List<FileSystemItem>();
            foreach (FileSystemItem item in FileSystemProvider.GetFolderContents(dir))
            {
                temp.Add(item);
                if (Util.Helper.IsFolder(item.Attributes))
                {
                    temp.AddRange(getAllSubPaths(item.FullPath));
                }
            }
            return temp;
        }

        private static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc(highBits + filetime.dwLowDateTime);
        }
    }
}
