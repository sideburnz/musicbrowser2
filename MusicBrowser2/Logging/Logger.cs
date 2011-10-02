using MusicBrowser.Util;
using System.Diagnostics;
using System;

namespace MusicBrowser.Logging
{
    static public class Logger
    {
        static ILogger _logger;

        public static void Error(Exception ex) { SelectedLogger.LogError(ex); }
        public static void Info(string message) { SelectedLogger.LogInfo(message); }
        public static void Debug(string message) { SelectedLogger.LogDebug(message); }
        public static void Verbose(string className, string endPoint) { SelectedLogger.LogVerbose(className, endPoint); }
        public static void Stats(string stats) { SelectedLogger.LogStats(stats); }

        static private ILogger SelectedLogger
        {
            get
            {
                try
                {
                    if (_logger == null)
                    {
                        string logDestination = Config.GetInstance().GetStringSetting("LogDestination");
                        switch (logDestination)
                        {
                            case "file":
                                {
                                    _logger = new FileLogger();
                                    break;
                                }
                            case "event log":
                                {
                                    _logger = new EventLogLogger();
                                    break;
                                }
                            default:
                                {
                                    _logger = new EventLogLogger();
                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!EventLog.SourceExists("MusicBrowser2"))
                    {
                        EventLog.CreateEventSource("MusicBrowser2", "Application");
                    }
                    EventLog.WriteEntry("MusicBrowser2",
                        "MusicBrowser 2 Fallback Logging\n\n" +
                        "Message: " + e.Message + "\n" +
//                        "Source: " + e.Source + "\n\n" +
                        e.StackTrace,
                        EventLogEntryType.Error);
                }
                return _logger;
            }
        }
    }
}
