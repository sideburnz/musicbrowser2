using System;
using System.Collections.Generic;
using System.Xml;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public enum ChartPeriod
    {
        Overall,
        Sevenday,
        Threemonth,
        Sixmonth,
        Twelvemonth,
    }

    public class ChartServiceDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }
        public ChartPeriod Period { get; set; }
        public int Hits { get; set; }

        //OUT
        public IEnumerable<LfmTrack> Tracks { get; set; }

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }

    public class ChartService : IWebService
    {
        LastFMWebProvider _provider;

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("LastFM.ChartService", "start");
#endif

            ChartServiceDTO localDTO = (ChartServiceDTO)dto;
            SortedDictionary<string, string> parms = new SortedDictionary<string, string>();

            parms.Add("method", "track.getTopTracks");
            parms.Add("user", Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
            parms.Add("limit", localDTO.Hits.ToString());

            switch (localDTO.Period)
            {
                case ChartPeriod.Overall:
                    parms.Add("period", "overall"); break;
                case ChartPeriod.Sevenday:
                    parms.Add("period", "7day"); break;
                case ChartPeriod.Sixmonth:
                    parms.Add("period", "6month"); break;
                case ChartPeriod.Threemonth:
                    parms.Add("period", "3month"); break;
                case ChartPeriod.Twelvemonth:
                    parms.Add("period", "12month"); break;
            }

            // this is a dummy URL for logging
            _provider.URL = "last.fm - chart - " + localDTO.Period.ToString();
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(_provider.ResponseBody);

            if (_provider.ResponseStatus == "200")
            {
                localDTO.Status = WebServiceStatus.Success;
                List<LfmTrack> temp = new List<LfmTrack>();

                foreach (XmlNode track in xmlDoc.SelectNodes("/lfm/toptracks/track"))
                {
                    LfmTrack l = new LfmTrack
                                     {
                                         Artist = ReadXmlNode(track, "artist/name"),
                                         Track = ReadXmlNode(track, "name"),
                                         Mbid = ReadXmlNode(track, "mbid")
                                     };
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

                Engines.Logging.LoggerEngineFactory.Debug(string.Format("Last.fm chart lookup for \"{0}\" returned this error - {1}", localDTO.Period.ToString(), localDTO.Error));
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
