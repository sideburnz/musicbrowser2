using System;
using System.IO;
using System.Reflection;
using MusicBrowser.Interfaces;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
using System.Text;

namespace MusicBrowser.CacheEngine
{
    public class MySqlCacheEngine : ICacheEngine
    {
        private MySqlConnection _conn;

        public void Delete(string key)
        {
            string sql = string.Format("DELETE FROM t_cache WHERE `key` = '{0}'", key);

            MySqlCommand cmd = new MySqlCommand(sql, GetConnection());
            cmd.ExecuteNonQuery();
        }

        public string FetchIfFresh(string key, DateTime comparer)
        {
            if (GetAge(key) < comparer) { return string.Empty; }

            string sql = string.Format("SELECT `value` FROM t_cache WHERE `key` = '{0}'", key);

            MySqlCommand cmd = new MySqlCommand(sql, GetConnection());
            return (string)cmd.ExecuteScalar();
        }

        public void Update(string key, string value)
        {
            string sql;

            if (Exists(key))
            {
                sql = string.Format("UPDATE t_cache SET `value`='{1}', `timestamp`=DEFAULT WHERE `key` = '{0}'", key, value.Replace("'", "''"));
            }
            else
            {
                sql = string.Format("INSERT INTO t_cache (`key`, `value`) VALUES ('{0}', '{1}')", key, value.Replace("'", "''"));
            }

            MySqlCommand cmd = new MySqlCommand(sql, GetConnection());
            cmd.ExecuteNonQuery();
        }

        public bool Exists(string key)
        {
            string sql = string.Format("SELECT COUNT(*) FROM t_cache WHERE `key` = '{0}'", key);

            MySqlCommand cmd = new MySqlCommand(sql, GetConnection());
            return Int64.Parse(cmd.ExecuteScalar().ToString()) == 1;
        }

        private DateTime GetAge(string key)
        {
            string sql = string.Format("SELECT `timestamp` FROM t_cache WHERE `key` = '{0}'", key);

            MySqlCommand cmd = new MySqlCommand(sql, GetConnection());
            return (DateTime)cmd.ExecuteScalar();
        }


        public MySqlConnection GetConnection()
        {
            if (_conn == null)
            {
                IniFile settings = new IniFile(System.IO.Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "\\settings.ini");

                string server = settings.ReadSetting(GetType().ToString(), "sever");
                string user = settings.ReadSetting(GetType().ToString(), "username");
                string password = settings.ReadSetting(GetType().ToString(), "password");

                string connStr = String.Format("server={0};user id={1}; password={2}; database=d_musicbrowser2; pooling=false", server, user, password);
                _conn = new MySqlConnection(connStr);
                _conn.Open();
            }
            return _conn;
        }



        public void Scavenge()
        {
            throw new NotImplementedException();
        }
    }



    public class IniFile
    {
        private readonly string _path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal, int size,string filePath);

        public IniFile(string iniPath)
        {
            _path = iniPath;
        }

        public void WriteSetting(string section,string key,string value)
        {
            WritePrivateProfileString(section,key,value,_path);
        }
       
        public string ReadSetting(string section,string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section,key,"",temp, 255, _path);
            return temp.ToString();
        }
    }
}

