using System;
using System.IO;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    class SQLiteLoader
    {
        private static bool _loadDll = Loaded();

        public static System.Reflection.Assembly sqliteAssembly;
        public static System.Reflection.Assembly SqliteResolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("System.Data.SQLite,"))
            {
                return sqliteAssembly;
            }
            return null;
        }

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        private static bool Loaded()
        {
            string dllfile = Path.Combine(Helper.ComponentFolder, String.Format("System.Data.SQLite.DLL"));
            if (!File.Exists(dllfile))
            {
                string sourceFile = Path.Combine(Helper.ComponentFolder, String.Format("System.Data.SQLite.DLL.{0}", Is64Bit ? 64 : 32));
                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, dllfile);
                }
            }
            sqliteAssembly = System.Reflection.Assembly.LoadFile(dllfile);
            AppDomain.CurrentDomain.AssemblyResolve += SqliteResolver;
            return true;
        }

        public static void Load()
        {
        }
    }
}
