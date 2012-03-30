using System.Collections.Generic;
using System.IO;
using System.Linq;

/******************************************************************************
 * 
 * JJ - 03-JAN-2011
 * 
 * Refactored provider to get a list of directories from a .vf file.
 * 
 * ***************************************************************************/

//image: C:\Program Files (x86)\MediaBrowser\MediaBrowser\Application.png
//folder: \\192.168.1.10\Media\Videos\TV
//folder: G:\RIPPED
//library: music
//sortorder: 000
//optimizefor: Music

namespace MusicBrowser.Providers.FolderItems
{
    /// <summary>
    /// provider to get a list of directories from a .vf file
    /// </summary>
    class VirtualFolderProvider : IFolderItemsProvider
    {
        #region IFolderItemsProvider Members

        /// <summary>
        /// Gets list of directories from a .vf file
        /// </summary>
        /// <param name="uri">path of .vf file</param>
        /// <returns>list of existing directories from .vf file</returns>
        public IEnumerable<string> GetItems(string uri)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(GetType().ToString(), "start");
#endif
            List<string> res = new List<string>();

            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }

            foreach (string line in GetFileContents(uri))
            {
                if (line.Length < 9) { continue; }

                if (line.StartsWith("folder:"))
                {
                    string thisPath = line.Substring(7).Trim();
                    if (Directory.Exists(thisPath))
                    {
                        res.AddRange(from p in FileSystemProvider.GetFolderContents(thisPath) where (p.Attributes & FileAttributes.Directory) == FileAttributes.Directory select p.FullPath);
                    }
                }
                if (line.StartsWith("library:"))
                {
                    try
                    {
                        string library = line.Substring(8).Trim();
                        WindowsLibraryProvider libProvider = new WindowsLibraryProvider();
                        IEnumerable<string> libLines = libProvider.GetItems(library);
                        res.AddRange(libLines.Where(Directory.Exists));
                    }
                    catch { }
                }
            }

            return res;
        }

        #endregion

        public static string GetImage(string uri)
        {
            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }
            foreach (string line in GetFileContents(uri))
            {
                if (line.Length < 9) { continue; }
                if (line.StartsWith("image:"))
                {
                    string thisPath = line.Substring(6).Trim();
                    thisPath = Path.Combine(Util.Config.GetInstance().GetStringSetting("Collections.Folder"), thisPath);
                    if (File.Exists(thisPath))
                    {
                        return thisPath;
                    }
                }
            }
            return null;
        }

        public static string GetTargetType(string uri)
        {
            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }
            foreach (string line in GetFileContents(uri))
            {
                if (line.Length < 9) { continue; }
                if (line.StartsWith("optimizefor:"))
                {
                    return line.Substring(12).Trim();
                }
            }
            return string.Empty;
        }

        public static IEnumerable<string> GetFolders(string uri)
        {
            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }
            return (from line in GetFileContents(uri) where line.Length >= 9 where line.StartsWith("folder:") select line.Substring(7).Trim()).ToList();
        }

        public static IEnumerable<string> GetLibraries(string uri)
        {
            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }
            return (from line in GetFileContents(uri) where line.Length >= 9 where line.StartsWith("library:") select line.Substring(8).Trim()).ToList();
        }

        public static string GetSortOrder(string uri)
        {
            if (Path.GetExtension(uri) != ".vf")
            {
                uri += ".vf";
            }
            foreach (string line in GetFileContents(uri))
            {
                if (line.Length < 9) { continue; }

                if (line.StartsWith("sortorder:"))
                {
                    return line.Substring(10).Trim();
                }
            }
            return string.Empty;
        }

        private static IEnumerable<string> GetFileContents(string path)
        {
            string line;
            List<string> rval = new List<string>();
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                rval.Add(line.ToLower());
            }
            file.Close();
            return rval;
        }

        public static void WriteVF(string vfPath, IEnumerable<string> paths, IEnumerable<string> libraries, string image, string optimizefor, string sortorder)
        {
            StreamWriter fs = File.CreateText(vfPath);
            if (!string.IsNullOrEmpty(image))
            {
                fs.WriteLine("image: " + image);
            }
            foreach (string path in paths)
            {
                fs.WriteLine("folder: " + path);
            }
            foreach (string library in libraries)
            {
                fs.WriteLine("library: " + library);
            }
            if (!string.IsNullOrEmpty(optimizefor))
            {
                fs.WriteLine("optimizefor: " + optimizefor);
            }
            if (!string.IsNullOrEmpty(sortorder))
            {
                fs.WriteLine("sortorder: " + sortorder);
            }
            fs.Flush();
            fs.Close();
        }

    }
}
