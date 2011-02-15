using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBrowser.Metadata.LastFM
{
    class TrackInfoService
    {
        public struct TrackInfoDTO
        {
        }

        public static void DoService(ref TrackInfoDTO DTO)
        {
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