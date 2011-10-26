using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MusicBrowser.Util;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Logging
{
    public sealed class EventLogLogger : ILoggingEngine
    {
        readonly bool _logErrors;
        readonly bool _logInfo;
        readonly bool _logDebug;
        readonly bool _logVerbose;
        readonly string _appName = string.Empty;

        #region constructors
        public EventLogLogger()
        {
            // cache the logging level information
            string logLevel = Config.GetInstance().GetStringSetting("Log.Level").ToLower();
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
            _appName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        #endregion

        #region ILogger Members

        void ILoggingEngine.LogError(Exception ex)
        {
            if (_logErrors)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Source: " + ex.Source);
                sb.AppendLine("Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    sb.AppendLine("Inner Exception");
                    sb.AppendLine("Source: " + ex.InnerException.Source);
                    sb.AppendLine("Message: " + ex.InnerException.Message);
                }
                if (ex.StackTrace != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Stack Trace");
                    sb.AppendLine(ex.StackTrace);
                }
                InnerLog(sb.ToString(), EventLogEntryType.Error);
            }
        }

        void ILoggingEngine.LogInfo(string message)
        {
            if (_logInfo)
            {
                InnerLog(message, EventLogEntryType.Information);
            }
        }

        void ILoggingEngine.LogDebug(string message)
        {
            if (_logDebug)
            {
                InnerLog(message, EventLogEntryType.Information);
            }
        }

        void ILoggingEngine.LogVerbose(string className, string endPoint)
        {
            if (_logVerbose)
            {
#if DEBUG
                InnerLog(string.Format("{0}: {1}", className, endPoint), EventLogEntryType.Information);
#endif
            }
        }

        void ILoggingEngine.LogStats(string stats)
        {
            InnerLog("Stats: " + stats, EventLogEntryType.Information);
        }

        #endregion

        private void InnerLog (string message, EventLogEntryType level)
        {
            if (!EventLog.SourceExists(_appName))
            {
                EventLog.CreateEventSource(_appName, "Application");
            }
            EventLog.WriteEntry(_appName, message, level);
        }
    }
}
