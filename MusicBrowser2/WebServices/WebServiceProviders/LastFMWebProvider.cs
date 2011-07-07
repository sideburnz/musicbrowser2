using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MusicBrowser.WebServices.Helper;
using MusicBrowser.WebServices.Interfaces;

namespace MusicBrowser.WebServices.WebServiceProviders
{
    class LastFMWebProvider : WebServiceProvider
    {
        IDictionary<string, string> _parms;

        #region constants
        // these keys are for MusicBrowser2, if you're reusing this code you
        // can get your own from this URL: http://www.last.fm/api
        private const string ApiKey = "c2145788b6b9008665559eb0dc4ae159";
        private const string ApiSecret = "e8f555849feb3e323e4ec12ae19904a7";
        #endregion

        public void SetParameters(IDictionary<string, string> parms)
        {
            _parms = parms;
            _parms.Add("api_key", ApiKey);
        }

        private static string GetMd5(string rawString)
        {
            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            Byte[] originalBytes = Encoding.Default.GetBytes(rawString);
            Byte[] encodedBytes = new MD5CryptoServiceProvider().ComputeHash(originalBytes);
            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }

        private static string GetSig(IDictionary<string, string> parms)
        {
            StringBuilder sbSig = new StringBuilder();
            foreach (string item in parms.Keys)
            {
                sbSig.Append(item + parms[item]);
            }
            sbSig.Append(ApiSecret);
            return GetMd5(sbSig.ToString());
        }

        public override bool ValidateParams()
        {
            //TODO: implement
            return true;
        }

        public override void Execute()
        {
#if DEBUG
            Logging.LoggerFactory.Verbose("LastFMWebProvider.Execute(" + URL + ")", "start");
#endif
            HttpProvider http = new HttpProvider();
            StringBuilder sb = new StringBuilder();

            sb.Append("http://ws.audioscrobbler.com/2.0/?");

            foreach(string item in _parms.Keys)
            {
                sb.Append(Externals.EncodeURL(item) + "=" + Externals.EncodeURL(_parms[item]) + "&");
            }

            sb.Append("api_sig=" + GetSig(_parms));
    
            http.Url = sb.ToString();
            http.Method = HttpProvider.HttpMethod.Get;

            if (!String.IsNullOrEmpty(RequestBody))
            {
                http.Body = RequestBody;
                http.Method = HttpProvider.HttpMethod.Post;
            }

            ResponseStatus = "System Error";

            // last.fm has a policy which is no more than 5 hits per second
            do { } while (!LFMThrottler.Hit());

            http.DoService();

            ResponseStatus = http.Status;
            ResponseBody = http.Response;
        }
    }

    /// <summary>
    /// Last.fm has rules about how much you can hammer their servers,
    /// this little class is meant to ensure that no more than five 
    /// requests per second are sent.
    /// </summary>
    static class LFMThrottler
    {
        private const int MaxHitsPerSecond = 5;
        private const int MsBetweenHits = 1000 / MaxHitsPerSecond;

        static DateTime _nextHit;

        public static bool Hit()
        {
            if (DateTime.Now <= _nextHit)
            {
                System.Threading.Thread.Sleep(MsBetweenHits);
                return false;
            }
            _nextHit = DateTime.Now.AddMilliseconds(MsBetweenHits);
            return true;
        }
    }
}
