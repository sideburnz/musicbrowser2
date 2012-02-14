using System;
using System.Collections.Generic;
using System.Xml;
using MusicBrowser.Engines.Logging;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class ArtistInfoServiceDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }
        public DateTime lastAccessed { get; set; }

        //DUAL
        public string MusicBrainzID { get; set; }
        public string Artist { get; set; }

        //OUT
        public int Plays { get; set; }
        public int Listeners { get; set; }
        public int TotalPlays { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }


    class ArtistInfoService : IWebService
    {
        LastFMWebProvider _provider;

        #region IWebService Members

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("LastFM.ArtistInfoService", "start");
#endif
            ArtistInfoServiceDTO localDTO = (ArtistInfoServiceDTO)dto;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "artist.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);

            if (String.IsNullOrEmpty(localDTO.MusicBrainzID))
            {
                parms.Add("artist", localDTO.Artist);
            }
            else
            {
                parms.Add("mbid", localDTO.MusicBrainzID);
            }

            // this is a dummy URL for logging
            _provider.URL = "last.fm - artist info - artist=" + localDTO.Artist;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_provider.ResponseBody);

            if (_provider.ResponseStatus == "200")
            {
                string lfmMBID = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/mbid", localDTO.MusicBrainzID);
                string lfmArtistName = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/name", localDTO.Artist);

                // is what was returned what we sent, allow minor differences for corrections (e.g. Beyonce => Beyoncé)
                if ((localDTO.MusicBrainzID == lfmMBID) || 
                    (Util.Helper.Levenshtein(localDTO.Artist, lfmArtistName) < 3))
                {
                    localDTO.MusicBrainzID = lfmMBID;
                    localDTO.Artist = lfmArtistName;
                    localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/bio/summary", localDTO.Summary));
                    localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/image[@size='large']").Replace("126", "174s");
                    localDTO.Plays = Int32.Parse(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/stats/userplaycount", "0"));
                    localDTO.TotalPlays = Int32.Parse(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/stats/playcount", "0"));
                    localDTO.Listeners = Int32.Parse(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/artist/stats/listeners", "0"));
                }
                else
                {
                    localDTO.Status = WebServiceStatus.Error;
                    localDTO.Error = "Match not close enough";
                    LoggerEngineFactory.Debug(string.Format("Last.fm artist look up for \"{0}\" but matched \"{1}\" instead", localDTO.Artist, lfmArtistName));
                }
            }
            else
            {
                localDTO.Status = WebServiceStatus.Warning;
                localDTO.Error = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/error");
                if (String.IsNullOrEmpty(localDTO.Error))
                {
                    localDTO.Status = WebServiceStatus.Error;
                }
#if DEBUG
                Engines.Logging.LoggerEngineFactory.Error(new Exception(string.Format("Last.fm artist look up for \"{0}\" returned this error - {1} [{2}]", localDTO.Artist, localDTO.Error, _provider.URL)));
#endif
            }
            return localDTO;
        }

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        #endregion
    }


}



//        public static void DoService(ref ArtistInfoDTO DTO)
//        {
//            LastFMProvider.LFMRequest request = new LastFMProvider.LFMRequest();
//            request.RequiresSignature = false;
//            request.Params = new SortedDictionary<string, string>();

//            request.Params.Add("method", "artist.getInfo");

//            if (String.IsNullOrEmpty(DTO.mbid))
//            {
//                request.Params.Add("autocorrect", "1");
//                request.Params.Add("artist", DTO.artist);
//            }
//            else
//            {
//                request.Params.Add("mbid", DTO.mbid);
//            }
//            request.Params.Add("username", DTO.username);

//            try
//            {
//                XmlDataDocument XMLDoc = new XmlDataDocument();
//                XMLDoc = LastFMProvider.AccessLastFM(request);

//                DTO.artist = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/name");
//                DTO.mbid = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/mbid");
//                DTO.plays = Int32.Parse("0" + LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/stats/plays"));
//                DTO.summary = Helper.StripHTML(LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/bio/summary"));

//                DTO.background = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/image[@size='extralarge']").Replace("/252/", "/_/");
//                DTO.thumb = LastFMProvider.getSingleXMLValue(XMLDoc, "/lfm/artist/image[@size='large']").Replace("/126/", "/300x300/");
//            }
//            catch { }
//        }

//    }
//}


////artist.getInfo

////artist (Required (unless mbid)] : The artist name
////mbid (Optional) : The musicbrainz id for the artist
////lang (Optional) : The language to return the biography in, expressed as an ISO 639 alpha-2 code.
////autocorrect[0|1] (Optional) : Transform misspelled artist names into correct artist names, returning the correct version instead. The corrected artist name will be returned in the response.
////username (Optional) : The username for the context of the request. If supplied, the user's playcount for this artist is included in the response.
////api_key (Required) : A Last.fm API key.