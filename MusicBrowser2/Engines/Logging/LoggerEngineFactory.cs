using MusicBrowser.Util;
using System.Diagnostics;
using System;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Logging
{
    static public class LoggerEngineFactory
    {
        static ILoggingEngine _logger;

        public static void Error(Exception ex) { GetEngine().LogError(ex); }
        public static void Info(string message) { GetEngine().LogInfo(message); }
        public static void Debug(string message) { GetEngine().LogDebug(message); }
        public static void Verbose(string className, string endPoint) { GetEngine().LogVerbose(className, endPoint); }
        public static void Stats(string stats) { GetEngine().LogStats(stats); }

        static private ILoggingEngine GetEngine()
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
                    "Source: " + e.Source + "\n\n" +
                    e.StackTrace,
                    EventLogEntryType.Error);
            }
            return _logger;
        }
    }
}
