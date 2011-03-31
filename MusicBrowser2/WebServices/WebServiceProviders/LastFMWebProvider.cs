using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.Providers;
using System.Security.Cryptography;

namespace MusicBrowser.WebServices.WebServiceProviders
{
    class LastFMWebProvider : WebServiceProvider
    {
        IDictionary<string, string> _parms;

        #region constants
        // these keys are for MusicBrowser2, if you're reusing this code you
        // can get your own from this URL: http://www.last.fm/api
        private const string API_KEY = "c2145788b6b9008665559eb0dc4ae159";
        private const string API_SECRET = "e8f555849feb3e323e4ec12ae19904a7";
        #endregion


        public void SetParameters(IDictionary<string, string> parms)
        {
            _parms = parms;
            _parms.Add("api_key", API_KEY);
        }

        private static string getMD5(string rawString)
        {
            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(rawString);
            Byte[] encodedBytes = new MD5CryptoServiceProvider().ComputeHash(originalBytes);
            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }

        private static string getSig(IDictionary<string, string> parms)
        {
            StringBuilder sbSig = new StringBuilder();
            foreach (string item in parms.Keys)
            {
                sbSig.Append(item + parms[item]);
            }
            sbSig.Append(API_SECRET);
            return getMD5(sbSig.ToString());
        }

        public override bool ValidateParams()
        {
            //TODO: implement
            return true;
        }

        public override void Execute()
        {
#if DEBUG
            Logging.Logger.Verbose("LastFMWebProvider.Execute(" + base.URL + ")", "start");
#endif
            HTTPProvider http = new HTTPProvider();
            StringBuilder sb = new StringBuilder();

            sb.Append("http://ws.audioscrobbler.com/2.0/?");

            foreach(string item in _parms.Keys)
            {
                sb.Append(item + "=" + _parms[item] + "&");
            }

            sb.Append("api_sig=" + getSig(_parms));
    
            http.URL = sb.ToString();
            http.Method = HTTPProvider.HTTPMethod.GET;

            if (!String.IsNullOrEmpty(base.RequestBody))
            {
                http.Body = base.RequestBody;
                http.Method = HTTPProvider.HTTPMethod.POST;
            }

            base.ResponseStatus = "500";

            // last.fm has a policy which is no more than 5 hits per second
            do { } while (!LFMThrottler.Hit());

            http.DoService();

            base.ResponseStatus = "200";
            base.ResponseBody = http.Response;
        }
    }

    static class LFMThrottler
    {
        static DateTime _nextHit;

        public static bool Hit()
        {
            if (DateTime.Now <= _nextHit)
            {
                System.Threading.Thread.Sleep(300);
                return false;
            }
            _nextHit = DateTime.Now.AddMilliseconds(250);
            return true;
        }
    }
}
