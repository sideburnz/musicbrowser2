using System;
using System.Text;
using System.Net;
using System.IO;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.WebServices.Helper
{
    public class HttpProvider
    {
        private string _url;
        private string _body;
        private HttpMethod _method;
        private string _response;
        private string _status;
        private DateTime _lastAccessed;

        public enum HttpMethod
        {
            Post,
            Get
        }

        public string Url { set { _url = value; } }
        public string Body { set { _body = value; } }
        public HttpMethod Method { set { _method = value; } }
        public DateTime LastUpdated { set { _lastAccessed = value; } }

        public string Response { get { return _response; } }
        public string Status { get { return _status; } }

        public void DoService()
        {
            HttpWebRequest request;
            _status = "Timeout";

            // set up the request
            try
            {
                request = (HttpWebRequest)WebRequest.Create(_url);
                request.Method = _method.ToString();
                request.Timeout = 5000;
                if (_lastAccessed > DateTime.Parse("01-JAN-2000")) { request.IfModifiedSince = _lastAccessed; }

                if (!String.IsNullOrEmpty(_body))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byte1 = new ASCIIEncoding().GetBytes(_body);
                    request.ContentLength = byte1.Length;
                    request.GetRequestStream().Write(byte1, 0, byte1.Length);
                }

                _response = null;
            }
            catch (Exception ex)
            {
                Exception e = new Exception("HTTP Provider - failed to set up HTTP request: " + _url, ex);
                LoggerEngineFactory.Error(e);
                throw e;
            }

#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(_url, "request");
#endif

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            _response = reader.ReadToEnd();
                        }
                    }
                }
                _status = "200";
            }
            catch (WebException e)
            {
                try
                {
                    _status = ((HttpWebResponse)e.Response).StatusCode.ToString();
                    Stream stream = e.Response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    _response = reader.ReadToEnd();
                }
                catch
                {
                    _status = "CATASTROPHIC ERROR";
                    _response = string.Empty;
                }
            }
            catch
            {
                _status = "Unknown Error";
                _response = string.Empty;
            }
        }

    }
}
