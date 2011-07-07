using System;
using MusicBrowser.WebServices.Helper;

namespace MusicBrowser.WebServices.Interfaces
{

    /// <summary>
    /// Implements a template pattern, the base class handles logging and parameter validation,
    /// although the rules for validation are handled by the implementation
    /// </summary>
    public abstract class WebServiceProvider
    {
        public string URL { get; set; }
        public string Method { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseStatus { get; set; }

        public abstract bool ValidateParams();
        public abstract void Execute();

        public void DoService()
        {
#if DEBUG
            Logging.LoggerFactory.Verbose("WebServices.WebServiceProvider.DoService", "start");
#endif
            bool loggingEnabled = Registry.Read("WebServicesFramework", "EnableLogging").ToLower() == "true";
            string uid = Guid.NewGuid().ToString();

            if (loggingEnabled)
            {
                Helper.Logging.LogRequest(uid, URL, Method);
            }
            if (!ValidateParams()) { throw new NullReferenceException(); }
            try
            {
                // clear the response vlaues;
                ResponseStatus = string.Empty;
                ResponseBody = string.Empty;

                Execute();
            }
            finally
            {
                if (loggingEnabled)
                {
                    Helper.Logging.LogResponse(uid, ResponseStatus, ResponseBody);
                }
            }
        }
    }
}
