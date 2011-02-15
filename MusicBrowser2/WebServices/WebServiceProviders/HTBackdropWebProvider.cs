using System;
using System.Text;
using System.Web;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.Providers;

/******************************************************************************
 * Home Theatre Backdrops access provider
 * ****************************************************************************
 * 
 * JJ - 16-DEC-2010
 * 
 * ***************************************************************************/

namespace MusicBrowser.WebServices.WebServiceProviders
{
    public class HTBackdropWebProvider : WebServiceProvider
    {
        public override bool ValidateParams()
        {
            //TODO: implement
            return true;
        }

        public override void Execute()
        {
            Logging.Logger.Verbose("HTBackdropWebProvider.Execute(" + base.URL + ")", "start");

            HTTPProvider http = new HTTPProvider();
            http.URL = base.URL;
            http.Method = HTTPProvider.HTTPMethod.GET;

            if (!String.IsNullOrEmpty(base.RequestBody)) 
            {
                http.Body = base.RequestBody; 
                http.Method = HTTPProvider.HTTPMethod.POST; 
            }

            base.ResponseStatus = "500";

            http.DoService();

            base.ResponseStatus = "200";
            base.ResponseBody = http.Response;
        }
    }
}
