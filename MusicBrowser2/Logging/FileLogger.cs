using System;
using System.IO;
using System.Text;
using MusicBrowser.Util;

namespace MusicBrowser.Logging
{
    public sealed class FileLogger : ILogger
    {
        readonly bool _logErrors;
        readonly bool _logInfo;
        readonly bool _logDebug;
        readonly bool _logVerbose;
        readonly string _logFile = string.Empty;

        static readonly object Padlock = new object();

        #region constructors
        public FileLogger()
        {
            // cache the logging level information
            string logLevel = Config.GetInstance().GetSetting("LogLevel").ToLower();
            // error is the default
            if (logLevel == "error")
            {
                _logErrors = true;
            }
            if (logLevel == "info")
            {
                _logInfo = true;
                _logErrors = true;
            }
            // debug is logging everything
            if (logLevel == "debug")
            {
                _logDebug = true;
                _logInfo = true;
                _logErrors = true;
            }
            if (logLevel == "verbose")
            {
                _logDebug = true;
                _logInfo = true;
                _logErrors = true;
                _logVerbose = true;
            }
            _logFile = Config.GetInstance().GetSetting("LogFile");
        }
        #endregion

        #region ILogger Members

        void ILogger.LogError(Exception ex)
        {
            if (_logErrors)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToShortDateString() + ", ");
                sb.Append(DateTime.Now.ToString("HH:mm:ss.ff") + ", ");
                sb.Append("Error, ");

                sb.Append(ex.Source + ", ");
                sb.Append(ex.GetType() + ", ");
                sb.Append("\"" + ex.Message + "\"");
                if (ex.InnerException != null)
                {
                    sb.Append(", Inner Exception, ");
                    sb.Append(ex.InnerException.Source + ", ");
                    sb.Append(ex.InnerException.GetType() + ", ");
                    sb.Append("\"" + ex.InnerException.Message + "\"");
                }
                if (ex.StackTrace != null)
                {
                    sb.Append(", Stack Trace, ");
                    sb.Append("\"" + ex.StackTrace.Replace("\r\n", "").Replace("\r\n", "") + "\"");
                }
                InnerLog(sb.ToString());
            }
        }

        void ILogger.LogInfo(string message)
        {
            if (_logInfo)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToShortDateString() + ", ");
                sb.Append(DateTime.Now.ToString("HH:mm:ss.ff") + ", ");
                sb.Append("Info, ");
                sb.Append("\"" + message + "\"");
                InnerLog(sb.ToString());
            }
        }

        void ILogger.LogDebug(string message)
        {
            if (_logDebug)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToShortDateString() + ", ");
                sb.Append(DateTime.Now.ToString("HH:mm:ss.ff") + ", ");
                sb.Append("Debug, ");
                sb.Append("\"" + message + "\"");
                InnerLog(sb.ToString());
            }
        }

        void ILogger.LogVerbose(string className, string endPoint)
        {
#if DEBUG
            if (_logVerbose)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToShortDateString() + ", ");
                sb.Append(DateTime.Now.ToString("HH:mm:ss.ff") + ", ");
                sb.Append("Verbose, ");
                sb.Append(className + ",");
                sb.Append(endPoint);
                InnerLog(sb.ToString());
            }
#endif
        }

        void ILogger.LogStats(Providers.Statistics stats)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToShortDateString() + ", ");
            sb.Append(DateTime.Now.ToString("HH:mm:ss.ff") + ", ");
            sb.Append("Stats, ");
            sb.Append("\"" + stats + "\"");
            InnerLog(sb.ToString());
        }

        #endregion

        private void InnerLog(string message)
        {
            // when debug level logging is on, file locks have caused random crashes
            try
            {
                lock (Padlock)
                {
                    StreamWriter fs = File.AppendText(_logFile);
                    fs.WriteLine(message);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch {}
        }
    }
}
