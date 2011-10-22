using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MusicBrowser.Util;
using System.Net;

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
            TextWriter text = new StringWriter(sb);
            XmlWriter writer = new XmlTextWriter(text);

            writer.WriteStartElement("Statistics");
            writer.WriteAttributeString("ID", Config.GetInstance().GetStringSetting("Telemetry.ID"));
            writer.WriteAttributeString("Version", Application.Version);

            foreach (string item in _stats.Keys)
            {
                writer.WriteElementString(item.Replace(" ", ""), _stats[item].ToString());
            }
            writer.WriteEndElement();
            writer.Close();

            return sb.ToString();
        }

        public static void Send()
        {
            if (Config.GetInstance().GetBooleanSetting("Telemetry.Participate"))
            {
                string data = GetReport();

                StreamWriter fs = File.CreateText(Path.Combine(Helper.AppLogFolder, "telemetry.xml"));
                fs.WriteLine(data);
                fs.Flush();
                fs.Close();

                WebClient client = new WebClient();
                client.UploadString("http://stats.musicbrowser2.com/submit.asp", data);
            }
        }

    }
}
