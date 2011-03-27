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
#if DEBUG
            Logging.Logger.Verbose("LastFM.AlbumInfoService", "start");
#endif
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

            // this is a dummy URL for logging
            _provider.URL = "last.fm - album info - album=" + localDTO.Album + "&artist=" + localDTO.Artist + "&mbid=" + localDTO.MusicBrowserID;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument XMLDoc = new XmlDocument();

            if (_provider.ResponseStatus != "200") { return localDTO; }

            XMLDoc.LoadXml(_provider.ResponseBody);
 
            localDTO.Album = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/name");
            localDTO.Artist = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/artist");
            localDTO.MusicBrowserID = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/mbid");
            DateTime rel;
            DateTime.TryParse(Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/releasedate"), out rel);
            if (rel > DateTime.MinValue) { localDTO.Release = rel; }
            localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/userplaycount"));
            localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image[@size='mega']");
            if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image[@size='extralarge']"); }
            if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image[@size='large']"); }
            if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image[@size='medium']"); }
            if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/image[@size='small']"); }
            localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(XMLDoc, "/lfm/album/wiki/summary"));

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


//  <?xml version="1.0" encoding="utf-8" ?> 
//- <lfm status="ok">
//- <album>
//  <name>Eleventh Avenue</name> 
//  <artist>Ammonia</artist> 
//  <id>2076889</id> 
//  <mbid>ea472e2e-a4ad-4494-b61f-c34196764864</mbid> 
//  <url>http://www.last.fm/music/Ammonia/Eleventh+Avenue</url> 
//  <releasedate>8 May 1998, 00:00</releasedate> 
//  <image size="small">http://images.amazon.com/images/P/B000009B7B.01.MZZZZZZZ.jpg</image> 
//  <image size="medium">http://images.amazon.com/images/P/B000009B7B.01.MZZZZZZZ.jpg</image> 
//  <image size="large">http://images.amazon.com/images/P/B000009B7B.01.MZZZZZZZ.jpg</image> 
//  <image size="extralarge">http://images.amazon.com/images/P/B000009B7B.01.MZZZZZZZ.jpg</image> 
//  <image size="mega">http://images.amazon.com/images/P/B000009B7B.01.MZZZZZZZ.jpg</image> 
//  <listeners>411</listeners> 
//  <playcount>6497</playcount> 
//  <userplaycount>69</userplaycount> 
//- <tracks>
//- <track rank="1">
//  <name>Eleventh Avenue</name> 
//  <duration>251</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Eleventh+Avenue</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="2">
//  <name>You're Not the Only One Who Feels This Way</name> 
//  <duration>231</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/You%27re+Not+the+Only+One+Who+Feels+This+Way</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="3">
//  <name>Keep On My Side</name> 
//  <duration>205</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Keep+On+My+Side</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="4">
//  <name>Monochrome</name> 
//  <duration>127</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Monochrome</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="5">
//  <name>Killswitch</name> 
//  <duration>266</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Killswitch</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="6">
//  <name>Baby Blue</name> 
//  <duration>188</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Baby+Blue</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="7">
//  <name>Wishing Chair</name> 
//  <duration>183</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Wishing+Chair</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="8">
//  <name>Keeping My Hands Tied</name> 
//  <duration>212</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Keeping+My+Hands+Tied</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="9">
//  <name>4711</name> 
//  <duration>218</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/4711</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="10">
//  <name>Yeah, Doin' It</name> 
//  <duration>193</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Yeah%2C+Doin%27+It</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="11">
//  <name>Afterglow</name> 
//  <duration>215</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Afterglow</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//- <track rank="12">
//  <name>Pipe Dream</name> 
//  <duration>216</duration> 
//  <mbid /> 
//  <url>http://www.last.fm/music/Ammonia/_/Pipe+Dream</url> 
//  <streamable fulltrack="0">0</streamable> 
//- <artist>
//  <name>Ammonia</name> 
//  <mbid>c1c4dcd3-78c4-44d8-9ad4-072facd11c82</mbid> 
//  <url>http://www.last.fm/music/Ammonia</url> 
//  </artist>
//  </track>
//  </tracks>
//  <toptags /> 
//  </album>
//  </lfm>