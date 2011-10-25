using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
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

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }

    public class ArtistImageService : IWebService
    {
        private WebServiceProvider _provider;
        private const string ApiKey = "bab7d8e83aff45941193e5f765b25051";

        public void SetProvider(WebServiceProvider provider)
        {
            _provider = provider;
        }

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
            ArtistImageServiceDTO localDTO = (ArtistImageServiceDTO)dto;
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose("HTBackdrop.ArtistImageService.Fetch(" + localDTO.ArtistName + localDTO.ArtistMusicBrainzID +")", "start");
#endif
            _provider.URL = BuildURL(localDTO);
            _provider.Method = "GET";

            localDTO.ThumbList = new List<string>();
            localDTO.BackdropList = new List<string>();

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
                // thumbs
                foreach (XmlNode node in xmlResult.SelectNodes("/search/images/image[aid=5]"))
                {
                    // make sure the item we've found matches the artist we were looking for, use Levenshtein distance

                    string id = string.Empty;
                    string title = string.Empty;

                    foreach(XmlNode ch in node.ChildNodes)
                    {
                        if (ch.Name == "id") { id = ch.InnerText; }
                        if (ch.Name == "title") { title = ch.InnerText; }
                    }

                    if (!String.IsNullOrEmpty(localDTO.ArtistMusicBrainzID) || (Util.Helper.Levenshtein(title, localDTO.ArtistName) < 3))
                    {
                        localDTO.ThumbList.Add("http://htbackdrops.com/api/" + ApiKey + "/download/" + id + "/thumbnail");
                    }
                }
            }
            catch { }

            try
            {
                // backgrounds
                foreach (XmlNode node in xmlResult.SelectNodes("/search/images/image[aid=1]"))
                {
                    // make sure the item we've found matches the artist we were looking for, use Levenshtein distance
                    string id = string.Empty;
                    string title = string.Empty;

                    foreach (XmlNode ch in node.ChildNodes)
                    {
                        if (ch.Name == "id") { id = ch.InnerText; }
                        if (ch.Name == "title") { title = ch.InnerText; }
                    }

                    if (!String.IsNullOrEmpty(localDTO.ArtistMusicBrainzID) || (Util.Helper.Levenshtein(title, localDTO.ArtistName) < 3))
                    {
                        localDTO.BackdropList.Add("http://htbackdrops.com/api/" + ApiKey + "/download/" + id + "/fullsize");
                    }
                }
            }
            catch { }

            return localDTO;
        }

        private static string BuildURL(ArtistImageServiceDTO dto)
        {
            StringBuilder url = new StringBuilder();
            url.Append("http://htbackdrops.com/api/" + ApiKey + "/searchXML?");
            url.Append("default_operator=and&");
            url.Append("fields=title,mb_name,mb_alias&");

            if (dto.GetBackdrops && dto.GetThumbs) { url.Append("aid=1,5&"); }
            else if (dto.GetBackdrops) { url.Append("aid=1&"); }
            else if (dto.GetThumbs) { url.Append("aid=5&"); }

            if (String.IsNullOrEmpty(dto.ArtistMusicBrainzID))
            {
                url.Append("keywords=" + HttpUtility.UrlEncode(dto.ArtistName));
            }
            else
            {
                url.Append("mbid=" + dto.ArtistMusicBrainzID);
            }

            return url.ToString();
        }
    }
}