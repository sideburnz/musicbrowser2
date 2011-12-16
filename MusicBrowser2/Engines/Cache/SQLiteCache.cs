using System;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Util;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
using System.Data.SQLite;

namespace MusicBrowser.Engines.Cache
{
    public class SQLiteCache : ICacheEngine
    {
        private const string SQL_CREATE_TABLE = "CREATE TABLE [t_Cache] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [value] TEXT NULL, [kind] Character(12), [title] TEXT)";
        private const string SQL_INSERT = "INSERT INTO [t_Cache] ([key], [value], [kind], [title]) VALUES(@1, @2, @3, @4)";
        private const string SQL_UPDATE = "UPDATE [t_Cache] SET [value] = @1, [title] = @3 WHERE [key] = @2";
        private const string SQL_DELETE = "DELETE FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_SELECT = "SELECT [kind], [value] FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_EXISTS = "SELECT COUNT([key]) FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_CLEAR = "DELETE FROM [t_Cache]";
        private const string SQL_SEARCH = @"SELECT [key] FROM [t_Cache] WHERE [kind] = @1 AND ([title] LIKE @2 OR [title] LIKE @3 OR [title] LIKE @4 OR [title] LIKE @5)";
        private const string SQL_SCAVENGE = "SELECT [value] FROM [t_Cache]";

        private object _lock = new object();
        private static string _file = Path.Combine(Config.GetInstance().GetStringSetting("Cache.Path"), "cache.db");

        public SQLiteCache()
        {
            SQLiteHelper.EstablishDatabase(_file, SQL_CREATE_TABLE);
        }

        public void Delete(string key)
        {
            string SQL = SQL_DELETE.Replace("@1", "'" + key + "'");
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            SQLiteHelper.ExecuteNonQuery(SQL, cnn);
            cnn.Close();
        }

        public baseEntity Fetch(string key)
        {
            if (Exists(key))
            {
                string SQL = SQL_SELECT.Replace("@1", "'" + key + "'");
                SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
                cnn.Open();
                Dictionary<string, object> res = SQLiteHelper.ExecuteRowQuery(SQL, cnn);
                cnn.Close();
                if (res == null) { return null; }
                return EntityPersistance.Deserialize((string)res["kind"], (string)res["value"]);
            }
            return null;
        }

        public void Update(baseEntity e)
        {
            string key = e.CacheKey;
            string value = e.Serialize();
            string kind = e.GetType().Name;
            string title = e.Title;

            if (Exists(key))
            {
                SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
                cnn.Open();
                SQLiteCommand cmdU = cnn.CreateCommand();
                cmdU.CommandText = SQL_UPDATE;
                cmdU.Parameters.AddWithValue("@2", key);
                cmdU.Parameters.AddWithValue("@1", value);
                cmdU.Parameters.AddWithValue("@3", title);
                cmdU.ExecuteNonQuery();
                cnn.Close();
            }
            else
            {
                SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
                cnn.Open();
                SQLiteCommand cmdI = cnn.CreateCommand();
                cmdI.CommandText = SQL_INSERT;
                cmdI.Parameters.AddWithValue("@1", key);
                cmdI.Parameters.AddWithValue("@2", value);
                cmdI.Parameters.AddWithValue("@3", kind);
                cmdI.Parameters.AddWithValue("@4", title);
                cmdI.ExecuteNonQuery();
                cnn.Close();
            }
        }

        public bool Exists(string key)
        {
            string SQL = SQL_EXISTS.Replace("@1", "'" + key + "'");
            Int64 rows;
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            rows = SQLiteHelper.ExecuteScalar<Int64>(SQL, cnn);
            cnn.Close();
            return rows != 0;
        }

        public void Scavenge()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            IEnumerable<string> results = SQLiteHelper.ExecuteQuery<string>(SQL_SCAVENGE, cnn);
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
            string SQL = SQL_CLEAR;
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            SQLiteHelper.ExecuteNonQuery(SQL, cnn);
            cnn.Close();
        }

        public IEnumerable<String> Search(string kind, string criteria)
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            IEnumerable<string> results = SQLiteHelper.ExecuteQuery<string>(SQL_SEARCH, cnn, kind, 
                criteria + "%", 
                "the " + criteria + "%",
                "a " + criteria + "%",
                "an " + criteria + "%");
            cnn.Close();
            return results;
        }
    }
}
