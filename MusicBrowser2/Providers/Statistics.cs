using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Providers
{
    public class Statistics
    {
        #region singleton
        private static Statistics _instance = null;
        private static readonly object Padlock = new object();

        public static Statistics GetInstance()
        {
            lock (Padlock)
            {
                if (_instance != null) return _instance;
                _instance = new Statistics();
                return _instance;
            }
        }
        #endregion

        Dictionary<string, int> _stats;

        Statistics()
        {
            _stats = new Dictionary<string, int>();
        }

        public void Hit(string key)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key]++;
            }
            else
            {
                _stats.Add(key, 1);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in _stats.Keys)
            {
                sb.Append(item + ": " + _stats[item].ToString() + " ");
            }
            return sb.ToString();
        }

    }
}
