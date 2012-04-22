using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.ViewState
{
    sealed class ContainerViewState : BaseViewState
    {
        private const string SqlCreateTable = "CREATE TABLE [t_ViewState] ([key] CHARACTER(64) PRIMARY KEY NOT NULL, [view] CHARACTER(16) NULL, [thumbsize] INTEGER NULL, [sortfield] CHARACTER(64) NULL, [ascending] INTEGER)";
        private const string SqlSelect = "SELECT [view], [thumbsize], [sortfield], [ascending] FROM [t_ViewState] WHERE [key]=@1";
        private const string SqlInsert = "INSERT INTO [t_ViewState] ([key], [view], [thumbsize], [sortfield], [ascending]) VALUES(@1, @2, @3, @4, @5)";
        private const string SqlDelete = "DELETE FROM [t_ViewState] WHERE [key]=@1";

        private static readonly string ViewStateFile = Config.GetInstance().GetStringSetting("ViewStateDatabase");
        private static readonly bool Loaded = SQLiteHelper.EstablishDatabase(ViewStateFile, SqlCreateTable);

        public ContainerViewState(string key) : base(key)
        {
            try
            {
                string sql = SqlSelect.Replace("@1", "'" + key + "'");
                SQLiteConnection cnn = SQLiteHelper.GetConnection(ViewStateFile);
                Dictionary<string, object> res = SQLiteHelper.ExecuteRowQuery(sql, cnn);
                if (res == null) { return; }

                View = (string)res["view"];
                ThumbSize = (int)(long)res["thumbsize"];
                SortField = (string)res["sortfield"];
                SortAscending = (bool)res["ascending"];
            }
            catch (Exception) { }
        }

        public override void InvertSort()
        {
            base.InvertSort();
            Commit();
        }

        public override void SetSortField(string field)
        {
            base.SetSortField(field);
            Commit();
        }

        public override void SetThumbSize(int size)
        {
            base.SetThumbSize(size);
            Commit();
        }

        public override void SetView(string view)
        {
            base.SetView(view);
            Commit();
        }

        private void Commit()
        {
            SQLiteConnection cnn = SQLiteHelper.GetConnection(ViewStateFile);

            string sql = SqlDelete.Replace("@1", "'" + key + "'");
            SQLiteHelper.ExecuteNonQuery(sql, cnn);

            SQLiteCommand cmdI = cnn.CreateCommand();
            cmdI.CommandText = SqlInsert;
            cmdI.Parameters.AddWithValue("@1", key);
            cmdI.Parameters.AddWithValue("@2", View);
            cmdI.Parameters.AddWithValue("@3", ThumbSize);
            cmdI.Parameters.AddWithValue("@4", SortField);
            cmdI.Parameters.AddWithValue("@5", SortAscending);
            cmdI.ExecuteNonQuery();
        }
    }
}
