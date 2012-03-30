using System;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Win32;

/******************************************************************************
 * 
 * This is a library that takes care of accessing Windows libraries.
 *
 *****************************************************************************/ 

namespace MusicBrowser.Providers.FolderItems
{
    public class WindowsLibraryProvider : IFolderItemsProvider
    {
        #region IFolderItemsProvider Members

        public System.Collections.Generic.IEnumerable<string> GetItems(string lib)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(GetType().ToString(), "start");
#endif

            bool runningOnExtender = Environment.UserName.ToLower().StartsWith("mcx");
            if (runningOnExtender)
            {
                Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                     ("Libraries cannot be used on extenders.",
                     "Error",
                     Microsoft.MediaCenter.DialogButtons.Ok,
                     100,
                     true);
                throw new Exception("Libraries cannot be used on extenders.");
            }

            XmlDocument libraryDfn;
            XmlNamespaceManager nsMgr;

            try
            {
                libraryDfn = GetLibraryDfn(lib);
                nsMgr = new XmlNamespaceManager(libraryDfn.NameTable);
                nsMgr.AddNamespace("lib", "http://schemas.microsoft.com/windows/2009/library");
            }
            catch (Exception ex)
            {
                throw new Exception("Windows Library not found", ex);
            }

            XmlNodeList nodes = libraryDfn.SelectNodes("lib:libraryDescription/lib:searchConnectorDescriptionList/lib:searchConnectorDescription/lib:simpleLocation/lib:url", nsMgr);

            return nodes.Cast<XmlNode>().Select(node => node.InnerText).Where(Directory.Exists);
        }

        private static string GetLibraryLocation(string lib)
        {
            //HKEY_USERS\[user]\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders\{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}
            RegistryKey pathKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\");
            string path = (pathKey.GetValue("{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}").ToString());
            return Path.Combine(path, lib + ".library-ms");
        }

        private static XmlDocument GetLibraryDfn(string lib)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(GetLibraryLocation(lib));
            return xml;
        }

        #endregion
    }
}
