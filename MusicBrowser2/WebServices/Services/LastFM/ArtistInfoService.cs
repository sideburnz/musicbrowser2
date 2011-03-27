using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class ArtistInfoServiceDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }

        //DUAL
        public string MusicBrowserID { get; set; }
        public string Artist { get; set; }

        //OUT
        public int Plays { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
    }


    class ArtistInfoService : IWebService
    {
        LastFMWebProvider _provider;

        #region IWebService Members

        public IWebServiceDTO Fetch(IWebServiceDTO DTO)
        {
#if DEBUG
            Logging.Logger.Verbose("LastFM.ArtistInfoService", "start");
#endif
            ArtistInfoServiceDTO localDTO = (ArtistInfoServiceDTO)DTO;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "artist.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);

            if (String.IsNullOrEmpty(localDTO.MusicBrowserID))
            {
                parms.Add("artist", localDTO.Artist);
            }
            else
            {
                parms.Add("mbid", localDTO.MusicBrowserID);
            }

            // this is a dummy URL for logging
            _provider.URL = "last.fm - artist info - artist=" + localDTO.Artist + "&mbid=" + localDTO.MusicBrowserID;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument XMLDoc = new XmlDocument();

            if (_provider.ResponseStatus != "200") { return localDTO; }

            XMLDoc.LoadXml(_provider.ResponseBody);

            localDTO.Artist = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/artist/name");
            localDTO.MusicBrowserID = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/artist/mbid");
            localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "/lfm/artist/stats/userplaycount"));
            localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(XMLDoc, "/lfm/artist/bio/summary"));
            localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/artist/image[@size='large']").Replace("126", "174s");

            return localDTO;
        }

        public void setProvider(WebServiceProvider provider)
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