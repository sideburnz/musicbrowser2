using System.Collections.Generic;
using System.IO;

/******************************************************************************
 * 
 * JJ - 03-JAN-2011
 * 
 * Refactored provider to get a list of directories from a .vf file.
 * 
 * ***************************************************************************/

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
            Logging.Logger.Verbose(this.GetType().ToString(), "start");
#endif
            foreach (string line in GetFileContents(uri))
            {
                if (!line.StartsWith("folder: ")) continue;
                string thisPath = line.Substring(8).Trim();
                if (Directory.Exists(thisPath))
                {
                    yield return thisPath;
                }
            }
        }

        #endregion

        private static IEnumerable<string> GetFileContents(string path)
        {
            string line;
            List<string> rval = new List<string>();
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                rval.Add(line);
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
