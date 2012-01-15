using System;
using MusicBrowser.Actions;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.PlugIns.Actions
{
    public class ActionPlayPopularLastFM : baseActionCommand
    {
        private const string LABEL = "Play Popular Last.fm";
        private const string ICON_PATH = "resx://LastFMProvider/LastFMProvider.Resources/IconLastFM";
        //,Culture=Neutral,Version=1.0.0.0,PublicKeyToken=a0d284a68f39c610
        //resx://LastFMProvider/LastFMProvider.Properties.Resources/IconLastFM
        //resx://LastFMProvider,Culture=Neutral,Version=1.0.0.0,PublicKeyToken=a0d284a68f39c610/LastFMProvider.Properties.Resources/IconLastFM
        //"res://LastFMProvider,Version=1.0.0.0,Culture=neutral,PublicKeyToken=a0d284a68f39c610!IconLastFM.png";
        

         public ActionPlayPopularLastFM(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            //Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
            Entity = entity;
        }

        public ActionPlayPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            //Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayPopularLastFM(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "tracks with the highest playcounts on Last.fm");
            //CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfm", entity), true);
            //MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
