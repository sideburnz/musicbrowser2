using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Interfaces;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using System.Data;

// in memory caching, intended to allow faster searches

namespace MusicBrowser.CacheEngine
{
    public class NearLineCache
    {
        private readonly DataTable _cache;

        #region singleton
        static NearLineCache _instance;

        NearLineCache()
        {
            _cache = new DataTable();

            _cache.Columns.Add("key", System.Type.GetType("System.String"));
            _cache.Columns.Add("path", System.Type.GetType("System.String"));
            _cache.Columns.Add("artist", System.Type.GetType("System.String"));
            _cache.Columns.Add("track", System.Type.GetType("System.String"));
            _cache.Columns.Add("favorite", System.Type.GetType("System.Boolean"));
            _cache.Columns.Add("rating", System.Type.GetType("System.Int32"));
            _cache.Columns.Add("serialized", System.Type.GetType("System.String"));

            _cache.Constraints.Add("PK", _cache.Columns["key"], true);

            _cache.CaseSensitive = false;
        }

        public static NearLineCache GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = new NearLineCache();
            return _instance;
        }
        #endregion

        public void Update(IEntity entity)
        {
            // we don't cache non-songs
            if (entity.Kind != EntityKind.Song) { return; }

            object[] rowData = new object[7];

            rowData[0] = entity.CacheKey;
            rowData[1] = entity.Path;
            rowData[2] = entity.ArtistName;
            rowData[3] = entity.TrackName;
            rowData[4] = entity.Favorite;
            rowData[5] = entity.Rating;
            rowData[6] = EntityPersistance.Serialize(entity);

            _cache.LoadDataRow(rowData, LoadOption.OverwriteChanges);
        }

        public IEnumerable<string> FindFavorites()
        {
            List<string> ret = new List<string>(100);
            DataRow[] rows = _cache.Select("rating > 4 OR favorite = True");
            foreach (DataRow row in rows)
            {
                ret.Add((string)row["path"]);
            }
            return ret;
        }

        public IEnumerable<string> FindByRating(int rating)
        {
            List<string> ret = new List<string>(100);
            DataRow[] rows = _cache.Select("rating >= " + rating);
            foreach (DataRow row in rows)
            {
                ret.Add((string)row["path"]);
            }
            return ret;
        }

        public IEnumerable<string> FindByPlays(int plays)
        {
            List<string> ret = new List<string>(100);
            DataRow[] rows = _cache.Select("plays >= " + plays);
            foreach (DataRow row in rows)
            {
                ret.Add((string)row["path"]);
            }
            return ret;
        }

        public IEnumerable<string> FindTrack(string artist, string track)
        {
            List<string> ret = new List<string>(100);
            DataRow[] rows = _cache.Select(String.Format("artist = '{0}' AND track = '{1}'", artist, track));
            foreach (DataRow row in rows)
            {
                ret.Add((string)row["path"]);
            }
            return ret;
        }

        public IEntity Fetch(string key)
        {
            DataRow dr = _cache.Rows.Find(key);
            if (dr != null)
            {
                Providers.Statistics.GetInstance().Hit("NLCache.hit");
                return EntityPersistance.Deserialize((string)dr["serialized"]);
            }
            return new Entities.Kinds.Unknown();
        }

    }
}
