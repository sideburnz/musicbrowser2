using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using MusicBrowser.Util;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public class SQLiteCache : ICacheEngine
    {
        private readonly string _file;
        private const string SQL_CREATE_TABLE = "CREATE TABLE [t_Cache] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [value] TEXT NULL, [kind] Character(12), [title] TEXT)";
        private const string SQL_INSERT = "INSERT INTO [t_Cache] ([key], [value], [kind], [title]) VALUES(@1, @2, @3, @4)";
        private const string SQL_UPDATE = "UPDATE [t_Cache] SET [value] = @1, [title] = @3 WHERE [key] = @2";
        private const string SQL_DELETE = "DELETE FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_SELECT = "SELECT [kind], [value] FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_EXISTS = "SELECT COUNT([key]) FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_CLEAR = "DELETE FROM [t_Cache]";
        private const string SQL_SEARCH = "SELECT [key] FROM [t_Cache] WHERE [kind] = @1 AND [title] LIKE @2";

        private object _lock = new object();

        public SQLiteCache()
        {
            _file = Path.Combine(Config.GetInstance().GetStringSetting("Cache.Path"), "cache.db");

            if (!File.Exists(_file))
            {
                SQLiteConnection.CreateFile(_file);
                ExecuteNonQuery(SQL_CREATE_TABLE);
            }
        }

        public void Delete(string key)
        {
            string SQL = SQL_DELETE.Replace("@1", "'" + key + "'"); ;
            ExecuteNonQuery(SQL);
        }

        public baseEntity Fetch(string key)
        {
            if (Exists(key))
            {
                string SQL = SQL_SELECT.Replace("@1", "'" + key + "'");
                Dictionary<string, object> res = ExecuteRowQuery(SQL);
                if (res == null) { return null; }
                return EntityPersistance.Deserialize((string)res["kind"], (string)res["value"]);
            }
            return null;
        }

        public void Update(baseEntity e)
        {
            string key = e.CacheKey;
            string value = EntityPersistance.Serialize(e);
            string kind = e.GetType().Name;
            string title = e.Title;

            if (Exists(key))
            {
                SQLiteConnection cnn = GetConnection();
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
                SQLiteConnection cnn = GetConnection();
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
            return ExecuteScalar<Int64>(SQL) != 0;
        }

        public void Scavenge()
        {
//            throw new NotImplementedException();
        }

        public void Clear()
        {
            string SQL = SQL_CLEAR;
            ExecuteNonQuery(SQL);
        }

        public IEnumerable<String> Search(string kind, string predicate)
        {
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            SQLiteCommand cmd = cnn.CreateCommand();
            cmd.CommandText = SQL_SEARCH;
            cmd.Parameters.AddWithValue("@1", kind);
            cmd.Parameters.AddWithValue("@2", predicate + "%");
            IEnumerable<string> results = ExecuteQuery<string>(cmd);
            cnn.Close();
            return results;
        }

        private int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        public t ExecuteScalar<t>(string sql)
        {
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return (t)value;
            }
            return default(t);
        }

        public Dictionary<string, object> ExecuteRowQuery(string sql)
        {
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
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
            cnn.Close();

            return ret;
        }

        public IEnumerable<T> ExecuteQuery<T>(SQLiteCommand cmd)
        {
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

        private SQLiteConnection GetConnection()
        {
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + _file);
            return cnn;
        }
    }
}
