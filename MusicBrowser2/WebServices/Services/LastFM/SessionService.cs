//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MusicBrowser.Providers;
//using System.Xml;

//namespace MusicBrowser.Metadata.LastFM
//{
//    class SessionService
//    {
//        public struct SessionDTO
//        {
//            //IN
//            public string AuthToken;
//            public string Username;

//            //OUT
//            public string SessionKey;
//            public string Response;
//        }

//        public static void DoService(ref SessionDTO Params)
//        {
//            //Authorise auth.getMobileSession
//            //username (Required) : The last.fm username.
//            //authToken (Required) : A 32-byte ASCII hexadecimal MD5 hash of the last.fm username and the user's password hash. i.e. md5(username + md5(password)), where '+' represents a concatenation.
//            //api_key (Required) : A Last.fm API key.
//            //api_sig (Required) : A Last.fm method signature.

//            Logging.LoggerFactory.SelectedLogger.LogVerbose("LastFM.SessionService", "entry");

//            LastFMProvider.LFMRequest request = new LastFMProvider.LFMRequest();
//            request.RequiresSignature = true;
//            request.Params = new SortedDictionary<string, string>();
//            request.Params.Add("method", "auth.getMobileSession");
//            request.Params.Add("authToken", Params.AuthToken);
//            request.Params.Add("username", Params.Username);

//            try
//            {
//                // default to no session, populate on success
//                Params.SessionKey = "not authenticated";

//                XmlDataDocument XMLDoc = new XmlDataDocument();
//                XMLDoc = LastFMProvider.AccessLastFM(request);
//                Params.Response = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/@status");
//                if (Params.Response == "ok")
//                {
//                    Params.SessionKey = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/session/key");
//                }
//            }
//            catch { }

//            Logging.LoggerFactory.SelectedLogger.LogVerbose("LastFM.SessionService", "exit");
//        }
//    }
//}
