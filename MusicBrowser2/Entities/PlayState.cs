using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Entities
{
    /// <summary>
    /// stores playstate (times played, progress etc) into a seperate file to the main cache which
    /// allows for a) this data to be shared and b) this data to survive a data refresh
    /// </summary>
    public class PlayState : IPlayState
    {
        private static readonly string PlayStateFile = Util.Config.GetInstance().GetStringSetting("PlayStateDatabase");
        private static readonly bool Loaded = SQLiteHelper.EstablishDatabase(PlayStateFile, SqlCreateTable);

        private const string SqlCreateTable = "CREATE TABLE [t_PlayState] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [timesplayed] INTEGER NULL, [progress] INTEGER NULL, [firstplayed] DATETIME NULL, [lastplayed] DATETIME NULL)";
        private const string SqlSelect = "SELECT [timesplayed], [progress], [firstplayed], [lastplayed] FROM [t_PlayState] WHERE [key]=@1";
        private const string SqlInsert = "INSERT INTO [t_PlayState] ([key], [timesplayed], [progress], [firstplayed], [lastplayed]) VALUES(@1, @2, @3, @4, @5)";
        private const string SqlDelete = "DELETE FROM [t_PlayState] WHERE [key]=@1";

        private long _timesplayed;
        private long _progress;
        private DateTime _firstplayed;
        private DateTime _lastplayed;

        private readonly string _key;

        public PlayState(string key)
        {
            _key = key;

            try
            {
                string sql = SqlSelect.Replace("@1", "'" + key + "'");
                SQLiteConnection cnn = SQLiteHelper.GetConnection(PlayStateFile);
                Dictionary<string, object> res = SQLiteHelper.ExecuteRowQuery(sql, cnn);
                if (res == null) { return; }

                _timesplayed = (long)res["timesplayed"];
                _progress = (long)res["progress"];
                _firstplayed = (DateTime)res["firstplayed"];
                _lastplayed = (DateTime)res["lastplayed"];
            }
            catch (Exception) { }
        }

        public long TimesPlayed
        {
            get { return _timesplayed; }
            set
            {
                if (value != _timesplayed)
                {
                    _timesplayed = value;
                    Commit();
                }
            }
        }
        public long Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    Commit();
                }
            }
        }
        public DateTime FirstPlayed
        {
            get { return _firstplayed; }
            set
            {
                if (value != _firstplayed)
                {
                    _firstplayed = value;
                    Commit();
                }
            }
        }
        public DateTime LastPlayed
        {
            get { return _lastplayed; } 
            set
            {
                if (value != _lastplayed)
                {
                    _lastplayed = value;
                    Commit();
                }
            }
        }

        public bool Played
        {
            get { return TimesPlayed > 0; }
            set
            {
                if (!value)
                {
                    TimesPlayed = 0;
                }
            }
        }

        /// <summary>
        /// does a few updates direct to values and then calls a single commit() to reduce DB calls
        /// </summary>
        public void Play()
        {
            _timesplayed++;
            _lastplayed = DateTime.Now;
            if (_firstplayed.Year < 2000)
            {
                _firstplayed = DateTime.Now;
            }
            Commit();
        }

        private void Commit()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(PlayStateFile);

            string sql = SqlDelete.Replace("@1", "'" + _key + "'");
            SQLiteHelper.ExecuteNonQuery(sql, cnn);

            SQLiteCommand cmdI = cnn.CreateCommand();
            cmdI.CommandText = SqlInsert;
            cmdI.Parameters.AddWithValue("@1", _key);
            cmdI.Parameters.AddWithValue("@2", _timesplayed);
            cmdI.Parameters.AddWithValue("@3", _progress);
            cmdI.Parameters.AddWithValue("@4", _firstplayed);
            cmdI.Parameters.AddWithValue("@5", _lastplayed);
            cmdI.ExecuteNonQuery();
        }
    }
}
