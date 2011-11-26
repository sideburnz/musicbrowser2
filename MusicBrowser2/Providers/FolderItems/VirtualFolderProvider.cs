using System.Collections.Generic;
using System.IO;

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
//sortorder: 000

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
            Engines.Logging.LoggerEngineFactory.Verbose(this.GetType().ToString(), "start");
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
                    string thisPath = line.Substring(8).Trim();
                    if (Directory.Exists(thisPath))
                    {
                        res.Add(thisPath);
                    }
                }
                if (line.StartsWith("library:"))
                {
                    try
                    {
                        string library = line.Substring(8).Trim();
                        WindowsLibraryProvider libProvider = new WindowsLibraryProvider();
                        IEnumerable<string> libLines = libProvider.GetItems(library);
                        foreach (string libLine in libLines)
                        {
                            if (Directory.Exists(libLine))
                            {
                                res.Add(libLine);
                            }
                        }
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
                    return line.Substring(8).Trim();
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

        public static void WriteVF(string vfPath, IEnumerable<string> paths)
        {
            StreamWriter fs = File.CreateText(vfPath);
            foreach (string path in paths)
            {
                fs.WriteLine("folder: " + path);
            }
            fs.Flush();
            fs.Close();
        }

    }
}
