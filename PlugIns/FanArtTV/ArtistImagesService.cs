using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using MusicBrowser.WebServices.Interfaces;

namespace MusicBrowser.WebServices.Services.FanArt
{
    public class ArtistImageServiceDTO : IWebServiceDTO
    {
        public string ArtistName { get; set; }
        public string MusicBrainzID { get; set; }

        public List<string> BackdropList { get; set; }
        public List<string> ClearLogoList { get; set; }

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }

    public class ArtistImageService : IWebService
    {
        private WebServiceProvider _provider;

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
            ArtistImageServiceDTO localDTO = (ArtistImageServiceDTO)dto;
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("FanArt.ArtistImageService.Fetch(" + localDTO.ArtistName + localDTO.MusicBrainzID +")", "start");
#endif
            _provider.URL = BuildURL(localDTO);
            _provider.Method = "GET";

            localDTO.BackdropList = new List<string>();
            localDTO.ClearLogoList = new List<string>();

            _provider.DoService();
            if (_provider.ResponseStatus != "200") { return localDTO; }

            XmlDocument xmlResult = new XmlDocument();

            try
            {
                xmlResult.LoadXml(_provider.ResponseBody);
            }
            catch { }

            try
            {
                // backgrounds
                foreach (XmlNode node in xmlResult.SelectNodes("/music/backgrounds/background"))
                {
                    localDTO.BackdropList.Add(node.InnerText);
                }
                // logos
                foreach (XmlNode node in xmlResult.SelectNodes("/music/clearlogos/clearlogo"))
                {
                    localDTO.ClearLogoList.Add(node.InnerText);
                }
            }
            catch { }

            return localDTO;
        }

        private static string BuildURL(ArtistImageServiceDTO dto)
        {
            StringBuilder url = new StringBuilder();
            url.Append("http://fanart.tv/api/music.php?id=");
            url.Append(dto.MusicBrainzID);
            return url.ToString();
        }
    }
}