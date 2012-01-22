using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    static class SQLiteHelper
    {
        private static bool LoadDLL = Loaded();

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
                    System.Reflection.Assembly.LoadFile(dllfile);
                }
            }
            return true;
        }

        public static bool EstablishDatabase(string file, string createSQL)
        {
            if (!File.Exists(file))
            {
                SQLiteConnection.CreateFile(file);
                SQLiteConnection cnn = GetConnection(file);
                cnn.Open();
                ExecuteNonQuery(createSQL, cnn);
                cnn.Close();
            }
            return true; // established
        }

        public static SQLiteConnection GetConnection(string file)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Data Source=" + file + ";");
            sb.Append("PRAGMA main.cache_size=-32;");
            sb.Append("PRAGMA main.temp_store = MEMORY;");
            sb.Append("PRAGMA main.page_size = 4096;");
            sb.Append("PRAGMA main.synchronous=OFF;");
            sb.Append("PRAGMA main.journal_mode=MEMORY;");

            SQLiteConnection cnn = new SQLiteConnection(sb.ToString());
                            
            return cnn;
        }

        public static int ExecuteNonQuery(string sql, SQLiteConnection cnn)
        {
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            return rowsUpdated;
        }

        public static t ExecuteScalar<t>(string sql, SQLiteConnection cnn)
        {
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            if (value != null)
            {
                return (t)value;
            }
            return default(t);
        }

        public static Dictionary<string, object> ExecuteRowQuery(string sql, SQLiteConnection cnn)
        {
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            SQLiteDataReader reader = mycommand.ExecuteReader();
            Dictionary<string, object> ret = null;
            if (reader.HasRows)
            {
                ret = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    ret.Add(reader.GetName(i), reader[i]);
                }
            }
            reader.Close();
            return ret;
        }

        public static IEnumerable<T> ExecuteQuery<T>(string sql, SQLiteConnection cnn, params string[] parms)
        {
            SQLiteCommand cmd = cnn.CreateCommand();
            cmd.CommandText = sql;
            for (int i = 0; i < parms.Length; i++)
            {
                cmd.Parameters.AddWithValue("@" + (i + 1), parms[i]);
            }
            SQLiteDataReader reader = cmd.ExecuteReader();

            List<T> ret = new List<T>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ret.Add((T)reader[0]);
                }
            }
            return ret;
        }
    }
}
