using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;
using MusicBrowser.WebServices.Services.LastFM;
using MusicBrowser.WebServices.Interfaces;
using MusicBrowser.WebServices.WebServiceProviders;

namespace MusicBrowser.Actions
{

    /// <summary>
    /// This is a first cut as a iTunes-Genius style playlist creator based on the Last.fm Track.getSimilar API
    /// </summary>
    public class ActionPlaySimilarTracks : baseActionCommand
    {
        private const string LABEL = "Play Similar Tracks";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconLastFM";

        public ActionPlaySimilarTracks(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public ActionPlaySimilarTracks()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlaySimilarTracks(entity);
        }

        public override void DoAction(Entity entity)
        {
            TrackSimilarDTO dto = new TrackSimilarDTO();
            dto.Artist = entity.ArtistName;
            dto.Track = entity.TrackName;

            TrackSimilarService service = new TrackSimilarService();
            WebServiceProvider webProvider = new LastFMWebProvider();
            service.SetProvider(webProvider);
            service.Fetch(dto);

            if (dto.Status == WebServiceStatus.Error) 
            {
                Models.UINotifier.GetInstance().Message = String.Format("error finding tracks similar to {0}", entity.Title);
                return;
            }
            if (dto.Tracks == null || dto.Tracks.Count() == 0)
            {
                Models.UINotifier.GetInstance().Message = String.Format("no tracks are similar to {0}", entity.Title);
                return;
            }

            int maxTracks = Util.Config.GetInstance().GetIntSetting("AutoPlaylistSize");
            int trackCount = 0;

            EntityCollection lib = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track);
            foreach (LfmTrack track in dto.Tracks)
            {
                Entity e = LocateTrack(lib, track);
                if (e == null)
                {
                    continue;
                }

                if (trackCount == 0)
                {
                    TransportEngineFactory.GetEngine().Play(false, e.Path);
                }
                else
                {
                    TransportEngineFactory.GetEngine().Play(true, e.Path);
                }
                trackCount++;
                if (trackCount > maxTracks)
                {
                    break;
                }
            }

            if (trackCount == 0)
            {
                Models.UINotifier.GetInstance().Message = String.Format("no tracks in your library are similar to {0}", entity.Title);
                return;
            }
            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }

        private Entity LocateTrack(EntityCollection lib, LfmTrack info)
        {
            return lib
                .Where(item => item.AlbumName.Equals(info.artist, StringComparison.CurrentCultureIgnoreCase) || 
                    item.TrackName.Equals(info.track, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();
        }
    }
}
