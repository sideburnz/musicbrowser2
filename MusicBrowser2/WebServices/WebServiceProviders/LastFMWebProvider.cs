using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.Providers;

namespace MusicBrowser.WebServices.WebServiceProviders
{
    class LastFMWebProvider : WebServiceProvider
    {
        IDictionary<string, string> _parms;

        public void SetParameters(IDictionary<string, string> parms)
        {
            _parms = parms;
        }

        public override bool ValidateParams()
        {
            //TODO: implement
            return true;
        }

        public override void Execute()
        {
            Logging.Logger.Verbose("LastFMWebProvider.Execute(" + base.URL + ")", "start");

            //TODO: api key
            //TODO: attribute hash

            HTTPProvider http = new HTTPProvider();

            StringBuilder sb = new StringBuilder();

            //sb.Append("http://ws.audioscrobbler.com/2.0/?");
            sb.Append("http://www.joocer.com?");

            foreach(string item in _parms.Keys)
            {
                sb.Append(item + "=" + _parms[item] + "&");
            }

            http.URL = sb.ToString();
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
