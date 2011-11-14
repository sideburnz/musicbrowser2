using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

        private readonly char[,] _replace = { 
                                                    { 'é', 'e' },
                                                    { 'ö', 'o' },
                                                    { 'í', 'i' },
                                                    { 'ř', 'r' },
                                                    { 'á', 'a' },
                                                    { 'Å', 'a' },
                                               };

        public void SetParameters(IDictionary<string, string> parms)
        {
            _parms = new Dictionary<string, string>();

            foreach (string key in parms.Keys)
            {
                string val = parms[key];
                for (int x = 0; x < _replace.GetLength(0); x++)
                {
                    val = val.Replace(_replace[x, 0], _replace[x, 1]);
                }
                _parms.Add(key, val);
            }

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
            return true;
        }

        public override void Execute()
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("LastFMWebProvider.Execute(" + URL + ")", "start");
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
            http.LastUpdated = LastAccessed;

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
    /// 
    /// this updated version will allow refreshes of data (upto about 1500 requests)
    /// to happen faster by allowing requests to be loaded to earlier in the 5 minute window
    /// for refreshes
    /// </summary>
    static class LFMThrottler
    {
        // actual limit is 5
        private const int MaxHitsPerSecond = 4;
        private static long hits = 0;
        private const int MsBetweenHits = 1000 / MaxHitsPerSecond;

        static DateTime _nextHit = DateTime.Now;
        private readonly static DateTime _start = DateTime.Now;

        public static bool Hit()
        {
            try
            {
                // allow the first few minutes to pump through requests quickly in order
                // for regular refreshes to happen quickly
                TimeSpan elapsed = DateTime.Now.Subtract(_start);
                if (elapsed.TotalMinutes < 3 && hits < 900) { hits++; return true; }

                // throttle longer sets of requests (longer than 3 mins or 900 requests)
                if (DateTime.Now <= _nextHit)
                {
                    System.Threading.Thread.Sleep(MsBetweenHits);
                    return false;
                }
                _nextHit = DateTime.Now.AddMilliseconds(MsBetweenHits);

                return true;
            }
            catch 
            {
                // thread.interrupt from the Background processor can cause the Sleep function here to throw an exception
                return false;
            }
        }
    }
}
