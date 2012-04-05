using MusicBrowser.Actions;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.PlugIns.Actions
{

    /// <summary>
    /// This is a first cut as a iTunes-Genius style playlist creator based on the Last.fm Track.getSimilar API
    /// </summary>
    public class ActionPlaySimilarTracks : baseActionCommand
    {
        private const string LABEL = "Play Similar Tracks (Last.fm)";
        private const string ICON_PATH = "resx://LastFMProvider/LastFMProvider.Resources/IconLastFM"; 

        public ActionPlaySimilarTracks(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlaySimilarTracks(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            //CommonTaskQueue.Enqueue(new PlaylistProvider("cmdsimilar", entity), true);
            //MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();

            // use "match" to ensure the results are a "good" match

            //<track>
            //<match>0.961879</match>
            //</track>
        }

    }
}
