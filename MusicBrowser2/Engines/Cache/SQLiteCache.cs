using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    public class SQLiteCache : ICacheEngine
    {
        private const string SqlCreateTable = "CREATE TABLE [t_Cache] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [value] TEXT NULL, [kind] Character(12), [title] TEXT)";
        private const string SqlInsert = "INSERT INTO [t_Cache] ([key], [value], [kind], [title]) VALUES(@1, @2, @3, @4)";
        private const string SqlUpdate = "UPDATE [t_Cache] SET [value] = @1, [title] = @3 WHERE [key] = @2";
        private const string SqlDelete = "DELETE FROM [t_Cache] WHERE [key]=@1";
        private const string SqlSelect = "SELECT [kind], [value] FROM [t_Cache] WHERE [key]=@1";
        private const string SqlExists = "SELECT COUNT([key]) FROM [t_Cache] WHERE [key]=@1";
        private const string SqlClear = "DELETE FROM [t_Cache]";
        private const string SqlSearch = "SELECT [key] FROM [t_Cache] WHERE [kind] = @1 AND ([title] LIKE @2 OR [title] LIKE @3)";
        private const string SqlScavenge = "SELECT [value] FROM [t_Cache]";
        private const string SqlCompress = "VACUUM";
        private const string SqlTypehits = "SELECT [kind], COUNT([key]) AS hits FROM [t_Cache] WHERE ([title] LIKE @1 OR [title] LIKE @2) GROUP BY [kind]";

        private static readonly string File = Path.Combine(Config.GetInstance().GetStringSetting("Cache.Path"), "entities.db");

        public SQLiteCache()
        {
            SQLiteHelper.EstablishDatabase(File, SqlCreateTable);
        }

        public void Delete(string key)
        {
            string sql = SqlDelete.Replace("@1", "'" + key + "'");
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            SQLiteHelper.ExecuteNonQuery(sql, cnn);
        }

        public baseEntity Fetch(string key)
        {
            string sql = SqlSelect.Replace("@1", "'" + key + "'");
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            Dictionary<string, object> res = SQLiteHelper.ExecuteRowQuery(sql, cnn);
            if (res == null) { return null; }
            return EntityPersistance.Deserialize((string)res["kind"], (string)res["value"]);
        }

        public void Update(baseEntity e)
        {
            string key = e.CacheKey;
            string value = e.Serialize();
            string kind = e.GetType().Name;
            string title = e.Title;

            if (Exists(key))
            {
                SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
                SQLiteCommand cmdU = cnn.CreateCommand();
                cmdU.CommandText = SqlUpdate;
                cmdU.Parameters.AddWithValue("@2", key);
                cmdU.Parameters.AddWithValue("@1", value);
                cmdU.Parameters.AddWithValue("@3", title);
                cmdU.ExecuteNonQuery();
            }
            else
            {
                SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
                SQLiteCommand cmdI = cnn.CreateCommand();
                cmdI.CommandText = SqlInsert;
                cmdI.Parameters.AddWithValue("@1", key);
                cmdI.Parameters.AddWithValue("@2", value);
                cmdI.Parameters.AddWithValue("@3", kind);
                cmdI.Parameters.AddWithValue("@4", title);
                cmdI.ExecuteNonQuery();
            }
        }

        public bool Exists(string key)
        {
            string sql = SqlExists.Replace("@1", "'" + key + "'");
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            long rows = SQLiteHelper.ExecuteScalar<Int64>(sql, cnn);
            return rows != 0;
        }

        public void Scavenge()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            cnn.Open();
            IEnumerable<string> results = SQLiteHelper.ExecuteQuery<string>(SqlScavenge, cnn);
            cnn.Close();
            foreach (string result in results)
            {
                // not everything is a folder, but everything should be able to be deserialized into a folder
                baseEntity e = EntityPersistance.Deserialize("Folder", result);
                FileSystemItem i = FileSystemProvider.GetItemDetails(e.Path);
                if (i.Attributes == 0)
                {
                    Delete(e.CacheKey);
                }
            }
        }

        public void Clear()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            SQLiteHelper.ExecuteNonQuery(SqlClear, cnn);
        }

        public void Compress()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            SQLiteHelper.ExecuteNonQuery(SqlCompress, cnn);
        }

        public IEnumerable<String> Search(string kind, string criteria)
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);
            IEnumerable<string> results = SQLiteHelper.ExecuteQuery<string>(SqlSearch, cnn, kind, 
                criteria + "%", 
                "% " + criteria + "%");
            return results;
        }

        public Dictionary<string, int> HitsByType(string criteria)
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(File);

            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = SqlTypehits;
            mycommand.Parameters.AddWithValue("@1", criteria + "%");
            mycommand.Parameters.AddWithValue("@2", "% " + criteria + "%");

            SQLiteDataReader reader = mycommand.ExecuteReader();
            Dictionary<string, int> ret = new Dictionary<string, int>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ret.Add(reader.GetString(0), reader.GetInt32(1));
                }
            }
            reader.Close();
            return ret;
        }
    }
}
