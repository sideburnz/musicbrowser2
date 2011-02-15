using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class AlbumInfoServiceDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }

        //DUAL
        public string Album { get; set; }
        public string MusicBrowserID { get; set; }
        public string Artist { get; set; }

        //OUT
        public DateTime Release { get; set; }
        public int Plays { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }
    }

    class AlbumInfoService : IWebService
    {
        LastFMWebProvider _provider;

        #region IWebService Members

        public IWebServiceDTO Fetch(IWebServiceDTO DTO)
        {
            AlbumInfoServiceDTO localDTO = (AlbumInfoServiceDTO)DTO;
            SortedDictionary<string,string> parms = new SortedDictionary<string,string>();

            parms.Add("method", "album.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);

            if (String.IsNullOrEmpty(localDTO.MusicBrowserID))
            {
                parms.Add("album", localDTO.Album);
                parms.Add("artist", localDTO.Artist);
            }
            else
            {
                parms.Add("mbid", localDTO.MusicBrowserID);
            }

            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument XMLDoc = new XmlDocument();

            if (_provider.ResponseStatus != "200") { return localDTO; }

            XMLDoc.LoadXml(_provider.ResponseBody);
 
            localDTO.Album = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/name");
            localDTO.Artist = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/artist");
            localDTO.MusicBrowserID = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/mbid");
            localDTO.Release = DateTime.Parse(Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/releasedate"));
            localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/playcount"));
            localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image/@small");
            localDTO.Summary = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/wiki/summary");

            return localDTO;
        }

        public void setProvider(WebServiceProvider provider)
        {
            _provider = (LastFMWebProvider)provider;
        }

        #endregion
    }
}

//album.getInfo

//album (Required (unless mbid)] : The album name
//artist (Required (unless mbid)] : The artist name
//mbid (Optional) : The musicbrainz id for the album
//lang (Optional) : The language to return the biography in, expressed as an ISO 639 alpha-2 code.
//autocorrect[0|1] (Optional) : Transform misspelled artist names into correct artist names, returning the correct version instead. The corrected artist name will be returned in the response.
//username (Optional) : The username for the context of the request. If supplied, the user's playcount for this album is included in the response.
//api_key (Required) : A Last.fm API key.


//<album>
//  <name>Believe</name>
//  <artist>Cher</artist>
//  <id>2026126</id>
//  <mbid>61bf0388-b8a9-48f4-81d1-7eb02706dfb0</mbid>
//  <url>http://www.last.fm/music/Cher/Believe</url>
//  <releasedate>    6 Apr 1999, 00:00</releasedate>
//  <image size="small">...</image>
//  <image size="medium">...</image>
//  <image size="large">...</image>
//  <listeners>47602</listeners>
//  <playcount>212991</playcount>
//  <toptags>
//    <tag>
//      <name>pop</name>
//      <url>http://www.last.fm/tag/pop</url>
//    </tag>
//    ...
//  </toptags>
//</album>