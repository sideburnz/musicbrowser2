using System;
using MusicBrowser.Engines.Logging;
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
        public DateTime LastAccessed { get; set; }

        public abstract bool ValidateParams();
        public abstract void Execute();

        public void DoService()
        {
            string uid = Guid.NewGuid().ToString();
#if DEBUG
            Logging.Logger.Verbose("WebServices.WebServiceProvider.DoService", "start");
            Helper.Logging.LogRequest(uid, URL, Method);
#endif
            if (!ValidateParams()) { throw new NullReferenceException(); }
            try
            {
                // clear the response vlaues;
                ResponseStatus = string.Empty;
                ResponseBody = string.Empty;

                Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ResponseStatus = "FATAL ERROR";
                ResponseBody = string.Empty;
            }
            finally
            {
#if DEBUG
                Helper.Logging.LogResponse(uid, ResponseStatus, ResponseBody);
#endif
            }
        }
    }
}
