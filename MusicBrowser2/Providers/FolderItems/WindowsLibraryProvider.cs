using System;
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
            Logging.Logger.Verbose(this.GetType().ToString(), "start");

            string sUserAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // really we should be reading this key in the registry {1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}
            // or even better a SpecialFolder type that reads from this registry key
            XmlDocument musicLib = new XmlDocument();
            string libDef = string.Format("{0}\\Microsoft\\Windows\\Libraries\\{1}.library-ms", sUserAppData, library);

            try
            {
                musicLib.Load(libDef);
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

        #endregion
    }
}
