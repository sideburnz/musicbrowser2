//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml;
//using Joocer.WebServices.Interfaces;

//namespace Joocer.WebServices.Services.LastFM
//{
//    class LastFMService : IWebService
//    {

//        // these keys are for MusicBrowser2, if you're reusing this code you
//        // can get your own from this URL: http://www.last.fm/api
//        private const string API_KEY = "c2145788b6b9008665559eb0dc4ae159";
//        private const string API_SECRET = "e8f555849feb3e323e4ec12ae19904a7";

//        //from http://www.ibt4im.com/?guid=20040923142406
//        public string getMD5(string rawString)
//        {
//            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
//            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(rawString);
//            Byte[] encodedBytes = new MD5CryptoServiceProvider().ComputeHash(originalBytes);
//            // Convert encoded bytes back to a 'readable' string
//            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
//        }

//        public string getSingleXMLValue(XmlDataDocument XML, string XPath)
//        {
//            if (XML == null)
//            {
//                return string.Empty;
//            }
//            try
//            {
//                string rval = XML.SelectSingleNode(XPath).InnerText.Trim();
//                if (rval.Length == 0)
//                {
//                    return string.Empty;
//                }
//                return rval;
//            }
//            catch
//            {
//                return string.Empty;
//            }
//        }

//        // AuthToken = md5(username + md5(password))
//        // the password should be MD5ed before it's passed here
//        public string getAuthToken(string username, string passwordHash)
//        {
//            return getMD5(username.ToLower() + passwordHash);
//        }


//        private WebServiceProvider _provider;

//        public void setProvider(WebServiceProvider provider)
//        {
//            _provider = provider;
//        }

//        public IWebServiceDTO Fetch(IWebServiceDTO DTO)
//        {
//            try
//            {
//                string API_SIG = string.Empty;
//                string API_URL = string.Empty;

//                Request.Params.Add("api_key", API_KEY);

//                if (Request.RequiresSignature)
//                {
//                    StringBuilder sbSig = new StringBuilder();
//                    foreach (string item in Request.Params.Keys)
//                    {
//                        sbSig.Append(item + Request.Params[item]);
//                    }
//                    sbSig.Append(API_SECRET);
//                    API_SIG = getMD5(sbSig.ToString());
//                }

//                StringBuilder sbURI = new StringBuilder("http://ws.audioscrobbler.com/2.0/?");
//                foreach (string item in Request.Params.Keys)
//                {
//                    sbURI.Append(item + "=" + HttpUtility.UrlEncode(Request.Params[item]) + "&").Replace("+", " ");
//                }
//                sbURI.Append("api_sig=" + API_SIG);
//                API_URL = sbURI.ToString();

//                _provider.RequestBody = string.Empty;
//                _provider.Method = "GET";
//                _provider.URL = API_URL;
//                _provider.DoService();



//                XmlDataDocument XMLDoc = new XmlDataDocument();
//                XMLDoc.LoadXml(HTTPRequest.Response);
//                return XMLDoc;
//            }
//            catch (Exception Ex)
//            {
//                Logging.LoggerFactory.SelectedLogger.LogError(new Exception("Last.FM request failed", Ex));
//            }

//            return null;
//        }
//    }
//}
