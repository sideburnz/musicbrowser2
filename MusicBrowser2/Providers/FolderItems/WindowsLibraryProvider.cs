using System;
using System.IO;
using Microsoft.Win32;
using System.Linq;
using System.Xml;
using MusicBrowser.Providers.FolderItems;
using MusicBrowser.Providers;

/******************************************************************************
 * 
 * This is a library that takes care of accessing the windows Music library.
 * The implementation is not the best, it relies of the user not changing the
 * default music library definition file.
 *
 *****************************************************************************/ 

namespace MusicBrowser.Providers.FolderItems
{
    public class WindowsLibraryProvider : IFolderItemsProvider
    {
        #region IFolderItemsProvider Members

        public System.Collections.Generic.IEnumerable<string> getItems(string library)
        {
#if DEBUG
            Logging.Logger.Verbose(this.GetType().ToString(), "start");
#endif
            
            XmlDocument musicLib = new XmlDocument();

            try
            {
                musicLib.Load(getLibraryLocation());
            }
            catch (Exception ex)
            {
                throw new Exception("Windows 7 music Library not found", ex);
            }

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(musicLib.NameTable);
            nsMgr.AddNamespace("lib", "http://schemas.microsoft.com/windows/2009/library");
            XmlNodeList nodes = musicLib.SelectNodes("lib:libraryDescription/lib:searchConnectorDescriptionList/lib:searchConnectorDescription/lib:simpleLocation/lib:url", nsMgr);

            foreach (XmlNode node in nodes)
            {
                string path = node.InnerText;
                if (System.IO.Directory.Exists(path))
                {
                    yield return path;
                }
            }
        }

        private static string getLibraryLocation()
        {
            //HKEY_USERS\[user]\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders\{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}
            RegistryKey pathKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\");
            string path = (pathKey.GetValue("{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}").ToString());
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FolderDescriptions\{2112AB0A-C86A-4ffe-A368-0DE96E47012E}\RelativePath
            RegistryKey fileKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderDescriptions\\{2112AB0A-C86A-4ffe-A368-0DE96E47012E}\\");
            string file = fileKey.GetValue("RelativePath").ToString();

            return Path.Combine(path, file);
        }

        #endregion
    }
}
