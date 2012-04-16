using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MusicBrowser.Util;

namespace MusicBrowser.Providers
{
    public static class Telemetry
    {
        private static readonly Dictionary<string, int> Stats = new Dictionary<string, int>();
        private static readonly DateTime Starttime = DateTime.Now;

        public static void Hit(string key)
        {
            Hit(key, 1);
        }

        public static void Hit(string key, int incrementor)
        {
            if (Stats.ContainsKey(key))
            {
                Stats[key] += incrementor;
            }
            else
            {
                Stats.Add(key, incrementor);
            }
        }

        private static string GetReport()
        {
            StringBuilder sb = new StringBuilder();
            TextWriter text = new StringWriter(sb);
            XmlWriter writer = new XmlTextWriter(text);

            writer.WriteStartElement("Telemetry");
            writer.WriteAttributeString("ID", Config.GetInstance().GetStringSetting("Telemetry.ID"));
            writer.WriteAttributeString("Version", Application.Version);
            writer.WriteAttributeString("ExecutionTime", Math.Truncate(DateTime.Now.Subtract(Starttime).TotalSeconds).ToString());

            foreach (string item in Stats.Keys)
            {
                writer.WriteStartElement("Action");
                writer.WriteAttributeString("key", item);
                writer.WriteAttributeString("value", Stats[item].ToString());
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

                WebServices.Helper.HttpProvider h = new WebServices.Helper.HttpProvider
                                                        {
                                                            Body =
                                                                "data=" + WebServices.Helper.Externals.EncodeURL(data),
                                                            Method = WebServices.Helper.HttpProvider.HttpMethod.Post,
                                                            Url = "http://stats.musicbrowser2.com:8080/submit.asp"
                                                        };
                h.DoService();

                if (h.Status != "200")
                {
                    Engines.Logging.LoggerEngineFactory.Error(new Exception("Telemetry failed: " + h.Response));
                }
            }
        }

    }
}
