using System;
using System.Collections.Generic;
using System.Xml;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class TrackInfoDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }
        public DateTime lastAccessed { get; set; }

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

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }

    class TrackInfoService : IWebService
    {
        LastFMWebProvider _provider;

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose("LastFM.TrackInfoService", "start");
#endif

            TrackInfoDTO localDTO = (TrackInfoDTO)dto;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "track.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);

            // Last.fm appears to have a defect with MusicBrainzID for tracks
            //if (String.IsNullOrEmpty(localDTO.MusicBrainzID))
            //{
                parms.Add("artist", localDTO.Artist);
                parms.Add("track", localDTO.Track);
            //}
            //else
            //{
            //    parms.Add("mbid", localDTO.MusicBrainzID);
            //}

            // this is a dummy URL for logging
            _provider.URL = "last.fm - track info - track=" + localDTO.Track + "  artist=" + localDTO.Artist;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(_provider.ResponseBody);

            if (_provider.ResponseStatus == "200")
            {
                localDTO.Status = WebServiceStatus.Success;
                localDTO.Track = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/track/name", localDTO.Track);
                localDTO.Artist = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/track/artist/name", localDTO.Artist);
                localDTO.MusicBrainzID = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/track/mbid", localDTO.MusicBrainzID);
                localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/track/bio/summary", localDTO.Summary));
                localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "lfm/track/userplaycount", "0"));
                localDTO.TotalPlays = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "lfm/track/playcount", "0"));
                localDTO.Listeners = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "lfm/track/listeners", "0"));
                localDTO.Loved = (Util.Helper.ReadXmlNode(xmlDoc, "lfm/track/userloved") == "1");
            }
            else
            {
                localDTO.Status = WebServiceStatus.Error;
                localDTO.Error = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/error");

//                Logging.Logger.Debug(string.Format("Last.fm track lookup for \"{0}\" returned this error - {1}", localDTO.Track, localDTO.Error));
            }
    
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