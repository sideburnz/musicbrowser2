using System.Collections.Generic;
using System.IO;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers;

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
        /// <param name="URI">path of .vf file</param>
        /// <returns>list of existing directories from .vf file</returns>
        public IEnumerable<string> getItems(string URI)
        {
            Logging.Logger.Verbose(this.GetType().ToString(), "start");

            foreach (string line in GetFileContents(URI))
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

        private IEnumerable<string> GetFileContents(string path)
        {
            string line;
            List<string> rval = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                rval.Add(line);
            }
            file.Close();
            return rval;
        }

        public static void WriteVF(string VFPath, IEnumerable<string> paths)
        {
            StreamWriter fs = File.CreateText(VFPath);
            foreach (string path in paths)
            {
                fs.WriteLine("folder: " + path);
            }
            fs.Flush();
            fs.Close();
        }

    }
}
