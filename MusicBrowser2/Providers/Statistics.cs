using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Xml;
using MusicBrowser.Util;
using System.Net;
using MusicBrowser.WebServices.Helper;

namespace MusicBrowser.Providers
{
    //TODO: extend to provider telemetry

    public static class Statistics
    {
        private static readonly Dictionary<string, int> _stats = new Dictionary<string, int>();
        private static readonly DateTime _starttime = DateTime.Now;

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
            writer.WriteAttributeString("ExecutionTime", Math.Truncate(DateTime.Now.Subtract(_starttime).TotalSeconds).ToString());

            foreach (string item in _stats.Keys)
            {
                writer.WriteStartElement("Action");
                writer.WriteAttributeString("key", item);
                writer.WriteAttributeString("value", _stats[item].ToString());
                writer.WriteEndElement();
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

                WebServices.Helper.HttpProvider h = new WebServices.Helper.HttpProvider();
                h.Body = "data=" + WebServices.Helper.Externals.EncodeURL(data);
                h.Method = WebServices.Helper.HttpProvider.HttpMethod.Post;
                h.Url = "http://stats.musicbrowser2.com/submit.asp";
                h.DoService();

                if (h.Status != "200")
                {
                    Engines.Logging.LoggerEngineFactory.Error(new Exception("Telemetry failed: " + h.Response));
                }
            }
        }

    }
}
