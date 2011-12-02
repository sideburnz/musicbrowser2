using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using System.Data.SQLite;
using System.Data;
using System.Data.Sql;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    public class SQLiteCache : ICacheEngine
    {
        private readonly string _file;
        private const string SQL_CREATE_TABLE = "CREATE TABLE [t_Cache] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [value] BLOB NULL)";
        private const string SQL_INSERT = "INSERT INTO [t_Cache] ([key], [value]) VALUES(@1, @2)";
        private const string SQL_UPDATE = "UPDATE [t_Cache] SET [value] = @1 WHERE [key]=@2";
        private const string SQL_DELETE = "DELETE FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_SELECT = "SELECT [value] FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_EXISTS = "SELECT COUNT([key]) FROM [t_Cache] WHERE [key]=@1";
        private const string SQL_CLEAR = "DELETE FROM [t_Cache]";

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

        public byte[] Fetch(string key)
        {
            if (Exists(key))
            {
                Providers.Statistics.Hit("SQLite.Hit");
                string SQL = SQL_SELECT.Replace("@1", "'" + key + "'");
                return ExecuteScalar<byte[]>(SQL);
            }
            Providers.Statistics.Hit("SQLite.Miss");
            return null;
        }

        public void Update(string key, byte[] value)
        {
            if (Exists(key))
            {
                SQLiteConnection cnn = GetConnection();
                cnn.Open();
                SQLiteCommand cmdU = cnn.CreateCommand();
                cmdU.CommandText = SQL_UPDATE;
                cmdU.Parameters.AddWithValue("@2", key);
                cmdU.Parameters.AddWithValue("@1", value);
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

        private SQLiteConnection GetConnection()
        {
            SQLiteConnection cnn = new SQLiteConnection("Data Source=" + _file);
            return cnn;
        }
    }
}
