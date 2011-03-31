using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class TrackInfoDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }

        //DUAL
        public string MusicBrainzID { get; set; }
        public string Artist { get; set; }
        public string Track { get; set; }

        //OUT
        public int Plays { get; set; }
        public int Listeners { get; set; }
        public int TotalPlays { get; set; }
        public string Summary { get; set; }
        public bool Loved { get; set; }
    }

    class TrackInfoService : IWebService
    {
        LastFMWebProvider _provider;

        public void setProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO DTO)
        {
#if DEBUG
            Logging.Logger.Verbose("LastFM.TrackInfoService", "start");
#endif

            TrackInfoDTO localDTO = (TrackInfoDTO)DTO;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "track.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);

            if (String.IsNullOrEmpty(localDTO.MusicBrainzID))
            {
                parms.Add("artist", localDTO.Artist);
                parms.Add("track", localDTO.Track);
            }
            else
            {
                parms.Add("mbid", localDTO.MusicBrainzID);
            }

            // this is a dummy URL for logging
            _provider.URL = "last.fm - track info - track=" + localDTO.Track + "  artist=" + localDTO.Artist + "&mbid=" + localDTO.MusicBrainzID;
            _provider.SetParameters(parms);
            _provider.DoService();

            if (_provider.ResponseStatus != "200") { return localDTO; }
            XmlDocument XMLDoc = new XmlDocument();

            XMLDoc.LoadXml(_provider.ResponseBody);

            //TODO: 2.2.1.10 confirm these are right
            localDTO.Track = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/track/name");
            localDTO.Artist = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/track/artist");
            localDTO.MusicBrainzID = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/track/mbid");
            localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(XMLDoc, "/lfm/track/bio/summary"));
            localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "lfm/track/userplaycount"));
            localDTO.TotalPlays = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "lfm/track/playcount"));
            localDTO.Listeners = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "lfm/track/listeners"));
            localDTO.Loved = (Util.Helper.ReadXmlNode(XMLDoc, "lfm/track/userloved") == "1");
            return localDTO;


        }
    }
}

//track.getInfo
//artist (Required (unless mbid)] : The artist name
//track (Required (unless mbid)] : The track name
//mbid (Optional) : The musicbrainz id for the track
//autocorrect[0|1] (Optional) : Transform misspelled artist and track names into correct artist and track names, returning the correct version instead. The corrected artist and track name will be returned in the response.
//username (Optional) : The username for the context of the request. If supplied, the user's playcount for this track and whether they have loved the track is included in the response.
//api_key (Required) : A Last.fm API key.

//this.Listeners = LFM.getSingleValue("lfm/track/listeners");
//this.PlayCount = LFM.getSingleValue("lfm/track/playcount");
//this.UserPlayCount = LFM.getSingleValue("lfm/track/userplaycount");
//this.UserLoved = LFM.getSingleValue("lfm/track/userloved") == "1";