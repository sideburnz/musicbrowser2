using System;

namespace MusicBrowser.Engines.Logging
{
    public interface ILoggingEngine
    {
        void LogError(Exception ex);
        void LogInfo(string className, string message);
        void LogDebug(string className, string message);
        void LogVerbose(string className, string endPoint);
        void LogStats(string stats);
    }
}
