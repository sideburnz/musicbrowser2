using System.Collections.Generic;
using System.Text;

namespace MusicBrowser.Providers
{
    //TODO: extend to provider telemetry

    public static class Statistics
    {
        private static readonly Dictionary<string, int> _stats = new Dictionary<string, int>();

        public static void Hit(string key)
        {
            Hit(key, 1);
        }

        public static void Hit(string key, int incrementor)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key] += incrementor;
            }
            else
            {
                _stats.Add(key, incrementor);
            }
        }

        public static string GetReport()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in _stats.Keys)
            {
                sb.Append(item + ": " + _stats[item].ToString() + " ");
            }
            return sb.ToString();
        }

        public static void Send()
        {

        }

    }
}
