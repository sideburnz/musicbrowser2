using System;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.MediaCenter.ListMaker;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/******************************************************************************
 * 
 * This is a library that takes care of accessing Windows libraries.
 *
 *****************************************************************************/ 

namespace MusicBrowser.Providers.FolderItems
{
    public class WindowsLibraryProvider : IFolderItemsProvider
    {

        //[DllImport(@"c:\Windows\ehome\ehuihlp.dll", CharSet = CharSet.Unicode)]
        //static extern int EhGetLocationsForLibrary(ref Guid knownFolderGuid, [MarshalAs(UnmanagedType.SafeArray)] out string[] locations);

        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);

        //Guid RecordedTVLibrary = new Guid("1A6FDBA2-F42D-4358-A798-B74D745926C5");
        //Guid MusicLibrary = new Guid("{2112AB0A-C86A-4ffe-A368-0DE96E47012E}");
        //Guid PicturesLibrary = new Guid("a990ae9f-a03b-4e80-94bc-9912d7504104");
        //Guid VideosLibrary = new Guid("491e922f-5643-4af4-a7eb-4e7a138d8174");

        private static string getpathKnown(Guid rfid)
        {
            IntPtr pPath;
            if (SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, out pPath) == 0)
            {
                string s = System.Runtime.InteropServices.Marshal.PtrToStringUni(pPath);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pPath);

                return s;
            }
            else return string.Empty;
        }


        #region IFolderItemsProvider Members

        public IEnumerable<string> GetItems(string lib)
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

            List<string> res = new List<string>();

            foreach (XmlNode node in nodes)
            {
                string item = node.InnerText;
                if (item.StartsWith("knownfolder:"))
                {
                    item = getpathKnown(new Guid(item.Substring(12)));
                }
                if (Directory.Exists(item))
                {
                    res.Add(item);
                }
            }

            return res;
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
