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
    public class SQLLiteCache : ICacheEngine
    {
        private readonly string _file;
        private const string KEY = "[%key%]";
        private const string VALUE = "[%value%]";
        private const string SQL_CREATE_TABLE = "CREATE TABLE [t_Cache] ([key] CHARACTER(64) PRIMARY KEY NOT NULL NOCASE, [value] TEXT NULL)";
        private const string SQL_INSERT = "INSERT INTO [t_Cache] ([key], [value]) VALUES('[%key%]', '[%value%]')";
        private const string SQL_UPDATE = "UPDATE [t_Cache] SET [value] = '[%value%]' WHERE WHERE [key]='[%key%]'";
        private const string SQL_DELETE = "DELETE FROM [t_Cache] WHERE [key]='[%key%]'";
        private const string SQL_SELECT = "SELECT [value] FROM [t_Cache] WHERE [key]='[%key%]'";
        private const string SQL_EXISTS = "SELECT [key] [t_Cache] WHERE [key]='[%key%]'";
        private const string SQL_CLEAR = "DELETE FROM [t_Cache]";

        private object _lock = new object();
        protected static System.Reflection.Assembly _assembly;

        public SQLLiteCache()
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
            string SQL = SQL_DELETE.Replace(KEY, key);
            ExecuteNonQuery(SQL);
        }

        public string Fetch(string key)
        {
            string SQL = SQL_SELECT.Replace(KEY, key);
            return ExecuteScalar<string>(SQL);
        }

        public void Update(string key, string value)
        {
            string SQL = SQL_UPDATE.Replace(KEY, key).Replace(VALUE, value);
            ExecuteNonQuery(SQL);
        }

        public bool Exists(string key)
        {
            string SQL = SQL_EXISTS.Replace(KEY, key);
            return ExecuteScalar<string>(SQL) == key;
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
            return new SQLiteConnection("Data Source=" + _file);
        }

        protected static void SqliteResolver()
        {
            string libraryPath = Path.Combine(Util.Helper.PlugInFolder, "System.Data.SQLite.dll");
            _assembly = System.Reflection.Assembly.LoadFile(libraryPath);
        }
    }
}
