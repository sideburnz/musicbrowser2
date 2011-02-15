using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using MusicBrowser.WebServices.Interfaces;

namespace MusicBrowser.WebServices.Services.HTBackdrop
{
    public class ArtistImageServiceDTO : IWebServiceDTO
    {
        public string ArtistName { get; set; }
        public string ArtistMusicBrainzID { get; set; }
        public bool GetThumbs { get; set; }
        public bool GetBackdrops { get; set; }

        public List<string> ThumbList { get; set; }
        public List<string> BackdropList { get; set; }
    }

    public class ArtistImageService : IWebService
    {
        private WebServiceProvider _provider;
        private const string API_KEY = "bab7d8e83aff45941193e5f765b25051";

        public void setProvider(WebServiceProvider provider)
        {
            _provider = provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO DTO)
        {
            ArtistImageServiceDTO localDTO = (ArtistImageServiceDTO)DTO;

            Logging.Logger.Verbose("HTBackdrop.ArtistImageService.Fetch(" + localDTO.ArtistName + localDTO.ArtistMusicBrainzID +")", "start");

            _provider.URL = BuildURL(localDTO);
            _provider.Method = "GET";

            localDTO.ThumbList = new List<string>();
            localDTO.BackdropList = new List<string>();

            _provider.DoService();
            if (_provider.ResponseStatus != "200") { Logging.Logger.Debug(_provider.ResponseStatus); return localDTO; }

            XmlDocument XmlResult = new XmlDocument();

            try
            {
                XmlResult.LoadXml(_provider.ResponseBody);
            }
            catch { }

            try
            {
                // thumbs
                foreach (XmlNode node in XmlResult.SelectNodes("/search/images/image[aid=5]/id"))
                {
                    localDTO.ThumbList.Add("http://htbackdrops.com/api/" + API_KEY + "/download/" + node.InnerText + "/thumbnail");
                }
            }
            catch { }

            try
            {
                // backgrounds
                foreach (XmlNode node in XmlResult.SelectNodes("/search/images/image[aid=1]/id"))
                {
                    localDTO.BackdropList.Add("http://htbackdrops.com/api/" + API_KEY + "/download/" + node.InnerText + "/fullsize");
                }
            }
            catch { }

            return localDTO;
        }

        private string BuildURL (ArtistImageServiceDTO dto)
        {
            StringBuilder URL = new StringBuilder();
            URL.Append("http://htbackdrops.com/api/" + API_KEY + "/searchXML?");
            URL.Append("default_operator=and&");
            URL.Append("fields=title,mb_name,mb_alias&");

            if (dto.GetBackdrops && dto.GetThumbs) { URL.Append("aid=1,5&"); }
            else if (dto.GetBackdrops) { URL.Append("aid=1&"); }
            else if (dto.GetThumbs) { URL.Append("aid=5&"); }

            if (String.IsNullOrEmpty(dto.ArtistMusicBrainzID))
            {
                URL.Append("keywords=" + HttpUtility.UrlEncode(dto.ArtistName));
            }
            else
            {
                URL.Append("mbid=" + dto.ArtistMusicBrainzID);
            }

            return URL.ToString();
        }
    }
}