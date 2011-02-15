//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MusicBrowser.Providers;
//using System.Xml;

//namespace MusicBrowser.Metadata.LastFM
//{
//    class ArtistInfoService
//    {
//        public struct ArtistInfoDTO
//        {
//            //IN
//            public string username;

//            //DUAL
//            public string mbid;
//            public string artist;

//            //OUT
//            public int plays;
//            public string summary;

//            public string thumb;
//            public string background;
//        }

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