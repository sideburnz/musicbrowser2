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
                Path.Combine(Util.Helper.AppLogFolder, DateTime.Now.ToString("yyyyMMdd") + " HTTP-REQUEST.log"));
        }

        public static void LogResponse(string uid, string status, string responseText)
        {
            // DateTime UID ResponseCode crlf Body crlf crlf
            InnerLog(DateTime.Now.ToString("HH:mm:ss") + "\t" + uid + "\t" + status + "\r\n" +
                responseText + "\r\n",
                Path.Combine(Util.Helper.AppLogFolder, DateTime.Now.ToString("yyyyMMdd") + " HTTP-RESPONSE.log"));
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
