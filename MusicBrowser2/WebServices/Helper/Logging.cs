using System;
using System.IO;

namespace MusicBrowser.WebServices.Helper
{
//    EnableHTTPLogging

    static class Logging
    {
        public static void LogRequest(string uid, string url, string method)
        {
            // DateTime UID crlf URL crlf BODY crlf crlf
            InnerLog(DateTime.Now.ToString("HH:mm:ss") + "\t" + uid + "\t" + method + "\r\n" +
                url + "\r\n",
                Path.Combine(Registry.Read("WebServicesFramework", "LogPath"), "HTTP-REQUEST-" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
        }

        public static void LogResponse(string uid, string status, string responseText)
        {
            // DateTime UID ResponseCode crlf Body crlf crlf
            InnerLog(DateTime.Now.ToString("HH:mm:ss") + "\t" + uid + "\t" + status + "\r\n" +
                responseText + "\r\n",
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
