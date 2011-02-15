using System;
using System.IO;

namespace MusicBrowser.WebServices.Helper
{
//    EnableHTTPLogging

    static class Logging
    {
        public static void LogRequest(string UID, string URL, string Method)
        {
            // DateTime UID crlf URL crlf BODY crlf crlf
            InnerLog(DateTime.Now.ToString("hh:mm:ss") + "\t" + UID + "\t" + Method + "\r\n" +
                URL + "\r\n",
                Path.Combine(Registry.Read("WebServicesFramework", "LogPath"), "HTTP-REQUEST-" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
        }

        public static void LogResponse(string UID, string Status, string ResponseText)
        {
            // DateTime UID ResponseCode crlf Body crlf crlf
            InnerLog(DateTime.Now.ToString("hh:mm:ss") + "\t" + UID + "\t" + Status + "\r\n" +
                ResponseText + "\r\n",
                Path.Combine(Registry.Read("WebServicesFramework", "LogPath"), "HTTP-RESPONSE-" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
        }

        private static void InnerLog(string message, string logFile)
        {
            try
            {
                StreamWriter fs = File.AppendText(logFile);
                fs.WriteLine(message);
                fs.Flush();
                fs.Close();
            }
            catch { }
        }
    }
}
