using System;
using System.Collections.Generic;
using System.Xml;
using MusicBrowser.Engines.Logging;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.WebServices.Services.LastFM
{
    public class AlbumInfoServiceDTO : IWebServiceDTO
    {
        //IN
        public string Username { get; set; }
        public DateTime LastAccessed { get; set; }
        public string Language { get; set; }

        //DUAL
        public string Album { get; set; }
        public string MusicBrainzID { get; set; }
        public string Artist { get; set; }

        //OUT
        public DateTime Release { get; set; }
        public int Plays { get; set; }
        public int Listeners { get; set; }
        public int TotalPlays { get; set; }
        public string Image { get; set; }
        public string Summary { get; set; }

        #region interface
        public WebServiceStatus Status { get; set; }
        public string Error { get; set; }
        #endregion
    }

    class AlbumInfoService : IWebService
    {
        LastFMWebProvider _provider;

        #region IWebService Members

        public IWebServiceDTO Fetch(IWebServiceDTO dto)
        {
#if DEBUG
            LoggerEngineFactory.Verbose("LastFM.AlbumInfoService", "start");
#endif
            AlbumInfoServiceDTO localDTO = (AlbumInfoServiceDTO)dto;
            SortedDictionary<string,string> parms = new SortedDictionary<string,string>();

            parms.Add("method", "album.getInfo");
            parms.Add("autocorrect", "1");
            parms.Add("username", localDTO.Username);
            parms.Add("lang", localDTO.Language);

            if (String.IsNullOrEmpty(localDTO.MusicBrainzID))
            {
                parms.Add("album", localDTO.Album);
                parms.Add("artist", localDTO.Artist);
            }
            else
            {
                parms.Add("mbid", localDTO.MusicBrainzID);
            }

            // this is a dummy URL for logging
            _provider.URL = "last.fm - album info - album=" + localDTO.Album + "&artist=" + localDTO.Artist;
            _provider.SetParameters(parms);
            _provider.DoService();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(_provider.ResponseBody);

            if (_provider.ResponseStatus == "200")
            {

                string lfmMbid = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/mbid", localDTO.MusicBrainzID);
                string lfmAlbumName = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/name", localDTO.Artist);

                if ((localDTO.MusicBrainzID == lfmMbid) || (Util.Helper.Levenshtein(localDTO.Album, lfmAlbumName) < 3))
                {
                    localDTO.Album = lfmAlbumName;
                    localDTO.Artist = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/artist", localDTO.Artist);
                    localDTO.MusicBrainzID = lfmMbid;

                    DateTime rel;
                    DateTime.TryParse(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/releasedate"), out rel);
                    if (rel > DateTime.MinValue) { localDTO.Release = rel; }

                    localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/image[@size='mega']");
                    if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/image[@size='extralarge']"); }
                    if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/image[@size='large']"); }
                    if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/image[@size='medium']"); }
                    if (string.IsNullOrEmpty(localDTO.Image)) { localDTO.Image = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/image[@size='small']"); }

                    localDTO.Summary = Util.Helper.StripHTML(Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/wiki/summary"));

                    localDTO.Plays = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/userplaycount"));
                    localDTO.TotalPlays = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/playcount"));
                    localDTO.Listeners = Int32.Parse("0" + Util.Helper.ReadXmlNode(xmlDoc, "/lfm/album/listeners"));
                }
                else
                {
                    localDTO.Status = WebServiceStatus.Warning;
                    localDTO.Error = "Match not close enough";
                    LoggerEngineFactory.Debug(string.Format("Last.fm album look up for \"{0}\" by \"{1}\" but matched \"{2}\" instead", localDTO.Album, localDTO.Artist, lfmAlbumName));
                }
            }
            else
            {
                localDTO.Status = WebServiceStatus.Warning;
                localDTO.Error = Util.Helper.ReadXmlNode(xmlDoc, "/lfm/error");
                if (String.IsNullOrEmpty(localDTO.Error))
                {
                    localDTO.Status = WebServiceStatus.Error;
                }
            }
            return localDTO;
        }

        public void SetProvider(WebServiceProvider provider)
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