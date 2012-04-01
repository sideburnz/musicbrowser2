using System;
using System.IO;
using System.Reflection;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    class SQLiteLoader
    {
        public static Assembly SqliteAssembly;
        public static Assembly SqliteResolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("System.Data.SQLite,"))
            {
                return SqliteAssembly;
            }
            return null;
        }

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        public static void Load()
        {
            try
            {
                string dllfile = Path.Combine("C:\\Windows\\eHome", String.Format("System.Data.SQLite.DLL"));
                if (!File.Exists(dllfile))
                {
                    string sourceFile = Path.Combine(Helper.ComponentFolder, String.Format("System.Data.SQLite.DLL.{0}", Is64Bit ? 64 : 32));
                    if (File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, dllfile);
                    }
                }
                SqliteAssembly = Assembly.Load(dllfile);
                AppDomain.CurrentDomain.AssemblyResolve += SqliteResolver;
            }
            catch (Exception ex)
            {
                Logging.LoggerEngineFactory.Error(ex);
            }
        }
    }
}
