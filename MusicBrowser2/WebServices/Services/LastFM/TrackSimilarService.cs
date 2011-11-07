using System;
using System.Collections.Generic;
using System.Xml;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public struct LfmTrack
    {
        //public string mbid;
        public string artist;
        public string track;
    }

    public class TrackSimilarDTO : IWebServiceDTO
    {
        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion

        //public string MusicBrainzID { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }

        public IEnumerable<LfmTrack> Tracks;
    }

    public class TrackSimilarService : IWebService
    {
        LastFMWebProvider _provider;

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("LastFM.TrackInfoService", "start");
#endif

            TrackSimilarDTO localDTO = (TrackSimilarDTO)dto;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "track.getsimilar");

            parms.Add("artist", localDTO.Artist);
            parms.Add("track", localDTO.Track);

            // this is a dummy URL for logging
            _provider.URL = "last.fm - similar tracks - track=" + localDTO.Track + "  artist=" + localDTO.Artist;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(_provider.ResponseBody);

            if (_provider.ResponseStatus == "200")
            {
                localDTO.Status = WebServiceStatus.Success;
                List<LfmTrack> temp = new List<LfmTrack>();

                foreach(XmlNode track in xmlDoc.SelectNodes("/lfm/similartracks/track"))
                {
                    LfmTrack l = new LfmTrack();
                    l.artist = ReadXmlNode(track, "artist/name");
                    l.track = ReadXmlNode(track, "name");
                 
                    temp.Add(l);
                }

                localDTO.Tracks = temp;
            }
            else
            {
                localDTO.Status = WebServiceStatus.Warning;
                localDTO.Error = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/error");
                if (String.IsNullOrEmpty(localDTO.Error))
                {
                    localDTO.Status = WebServiceStatus.Error;
                }

                //                Engines.Logging.LoggerEngineFactory.Debug(string.Format("Last.fm track lookup for \"{0}\" returned this error - {1}", localDTO.Track, localDTO.Error));
            }

            return localDTO;

        }

        private static string ReadXmlNode(XmlNode parent, string xPath)
        {
            var selectSingleNode = parent.SelectSingleNode(xPath);
            if (selectSingleNode != null)
            {
                return selectSingleNode.InnerText;
            }
            return string.Empty;
        }
    }
}


//Params

//track (Required (unless mbid)] : The track name
//artist (Required (unless mbid)] : The artist name
//mbid (Optional) : The musicbrainz id for the track
//autocorrect[0|1] (Optional) : Transform misspelled artist and track names into correct artist and track names, returning the correct version instead. The corrected artist and track name will be returned in the response.
//limit (Optional) : Maximum number of similar tracks to return
//api_key (Required) : A Last.fm API key.

//<lfm status="ok">
//<similartracks track="Believe" artist="Cher">
//<track>
//<name>Strong Enough</name>
//<playcount>389758</playcount>
//<mbid/>
//<match>1</match>
//<url>http://www.last.fm/music/Cher/_/Strong+Enough</url>
//<streamable fulltrack="0">1</streamable>
//<duration>239000</duration>
//<artist>
//<name>Cher</name>
//<mbid>bfcc6d75-a6a5-4bc6-8282-47aec8531818</mbid>
//<url>http://www.last.fm/music/Cher</url>
//</artist>
//<image size="small">http://userserve-ak.last.fm/serve/34s/68751616.png</image>
//<image size="medium">http://userserve-ak.last.fm/serve/64s/68751616.png</image>
//<image size="large">http://userserve-ak.last.fm/serve/126/68751616.png</image>
//<image size="extralarge">
//http://userserve-ak.last.fm/serve/300x300/68751616.png
//</image>
//</track>