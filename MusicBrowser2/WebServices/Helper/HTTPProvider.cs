using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace MusicBrowser.Providers
{
    class HTTPProvider
    {
        private string _URL;
        private string _body;
        private HTTPMethod _method;
        private string _response = null;

        public enum HTTPMethod
        {
            POST,
            GET
        }

        public HTTPProvider() {}

        public string URL { set { _URL = value; } }
        public string Body { set { _body = value; } }
        public HTTPMethod Method { set { _method = value; } }

        public string Response
        {
            get
            {
                return _response;
            }
        }

        public void DoService()
        {
            Logging.Logger.Debug(String.Format("sending request to: {0}", _URL));

            byte[] byte1;
            HttpWebRequest request;
            WebResponse response;
            HttpWebResponse HTTPresponse;

            // set up the request
            try
            {
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = _method.ToString();
                request.Timeout = 2000;

                if (!String.IsNullOrEmpty(_body))
                {
                    byte1 = new ASCIIEncoding().GetBytes(_body);
                    request.ContentLength = byte1.Length;
                    request.GetRequestStream().Write(byte1, 0, byte1.Length);
                }

                _response = null;
            }
            catch (Exception Ex)
            {
                Exception e = new Exception("HTTP Provider - failed to set up HTTP request: " + _URL, Ex);
                Logging.Logger.Error(e);
                throw e;
            }

            try
            {
                response = request.GetResponse();
                HTTPresponse = (HttpWebResponse)response;
            }
            catch (Exception Ex)
            {
                Exception e = new Exception(String.Format("HTTP Provider - HTTP request failed: {0}", _URL), Ex);
                //Logging.LoggerFactory.SelectedLogger.LogError(e);
                throw e;
            }

            try
            {
                if (HTTPresponse.StatusCode == HttpStatusCode.OK)
                {
                    // load the response into an XML document
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    _response = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                else
                {
                    throw new IOException(String.Format("HTTP Provider - HTTP error response. Response: {0} URL: {1}", HTTPresponse.StatusCode, _URL));
                }
            }
            catch (IOException IOE)
            {
                throw;
            }
            catch (Exception Ex)
            {
                Exception e = new Exception(String.Format("HTTP Provider - Processing HTTP response failed: {0}", _URL), Ex);
                //Logging.LoggerFactory.SelectedLogger.LogError(e);
                throw e;
            }
        }

    }
}
