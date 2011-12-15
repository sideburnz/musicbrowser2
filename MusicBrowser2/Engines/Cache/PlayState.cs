using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Entities;
using System.Data.SQLite;
using System.IO;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Cache
{
    static class PlayState
    {
        private const string SQL_CREATE_TABLE = "CREATE TABLE [t_PlayState] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [played] DATETIME NULL, [progress] INT)";
        private const string SQL_EXISTS = "SELECT COUNT([key]) FROM [t_PlayState] WHERE [key]=@1";
        private const string SQL_INSERT = "INSERT INTO [t_PlayState] ([key], [played], [progress]) VALUES(@1, @2, @3)";
        private const string SQL_UPDATE = "UPDATE [t_PlayState] SET [played] = @2, [progress] = @3 WHERE [key] = @1";
        private const string SQL_FETCH_PLAYED = "SELECT [played] FROM [t_PlayState] WHERE [key]=@1";
        private const string SQL_FETCH_PROGRESS = "SELECT [progress] FROM [t_PlayState] WHERE [key]=@1";

        private static string _file = Path.Combine(Config.GetInstance().GetStringSetting("Cache.Path"), "playstate.db");
   
        public static void MarkPlayed(string key, int progress)
        {
            SQLiteHelper.EstablishDatabase(_file, SQL_CREATE_TABLE);

            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            SQLiteCommand cmd = cnn.CreateCommand();
            if (Exists(key))
            {
                cmd.CommandText = SQL_UPDATE;
            }
            else
            {
                cmd.CommandText = SQL_INSERT;
            }
            cmd.Parameters.AddWithValue("@1", key);
            cmd.Parameters.AddWithValue("@2", DateTime.Now);
            cmd.Parameters.AddWithValue("@3", progress);
            cmd.ExecuteNonQuery();
            cnn.Close();
        }

        private static bool Exists(string key)
        {
            SQLiteHelper.EstablishDatabase(_file, SQL_CREATE_TABLE);

            string SQL = SQL_EXISTS.Replace("@1", "'" + key + "'");
            Int64 rows;
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            rows = SQLiteHelper.ExecuteScalar<Int64>(SQL, cnn);
            cnn.Close();
            return rows != 0;
        }

        public static DateTime LastPlayed(string key)
        {
            SQLiteHelper.EstablishDatabase(_file, SQL_CREATE_TABLE);

            string SQL = SQL_FETCH_PLAYED.Replace("@1", "'" + key + "'");
            DateTime timestamp;
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            timestamp = SQLiteHelper.ExecuteScalar<DateTime>(SQL, cnn);
            cnn.Close();
            return timestamp;
        }

        public static int Progress(string key)
        {
            SQLiteHelper.EstablishDatabase(_file, SQL_CREATE_TABLE);

            string SQL = SQL_FETCH_PROGRESS.Replace("@1", "'" + key + "'");
            Int64 progress;
            SQLiteConnection cnn = SQLiteHelper.GetConnection(_file);
            cnn.Open();
            progress = SQLiteHelper.ExecuteScalar<Int64>(SQL, cnn);
            cnn.Close();
            return (int)progress;
        }
    }
}
