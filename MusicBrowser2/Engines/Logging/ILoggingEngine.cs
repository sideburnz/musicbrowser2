using System;

namespace MusicBrowser.Engines.Logging
{
    public interface ILoggingEngine
    {
        void LogError(Exception ex);
        void LogInfo(string message);
        void LogDebug(string message);
        void LogVerbose(string className, string endPoint);
        void LogStats(string stats);
    }
}
