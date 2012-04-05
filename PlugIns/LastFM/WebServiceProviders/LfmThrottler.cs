using System;

namespace MusicBrowser.WebServices.WebServiceProviders
{
    /// <summary>
    /// Last.fm has rules about how much you can hammer their servers,
    /// this little class is meant to ensure that no more than five 
    /// requests per second are sent.
    /// 
    /// this updated version will allow refreshes of data (upto about 1500 requests)
    /// to happen faster by allowing requests to be loaded to earlier in the 5 minute window
    /// for refreshes
    /// </summary>
    static class LfmThrottler
    {
        // actual limit is 5
        private const int MaxHitsPerSecond = 4;
        private static long _hits;
        private const int MsBetweenHits = 1000 / MaxHitsPerSecond;

        static DateTime _nextHit = DateTime.Now;
        private readonly static DateTime Start = DateTime.Now;

        public static bool Hit()
        {
            try
            {
                // allow the first few minutes to pump through requests quickly in order
                // for regular refreshes to happen quickly
                TimeSpan elapsed = DateTime.Now.Subtract(Start);
                if (elapsed.TotalMinutes < 3 && _hits < 900) { _hits++; return true; }

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