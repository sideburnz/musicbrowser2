using Microsoft.MediaCenter;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    public class ActionShowNowPlaying : baseActionCommand
    {
        private const string LABEL = "Show Now Playing";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionShowNowPlaying(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionShowNowPlaying()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowKeyboard(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            ITransportEngine t = TransportEngineFactory.GetEngine();
            if (t.HasBespokeNowPlaying)
            {
                if (TransportEngineFactory.GetEngine().ShowNowPlaying())
                {
                    return;
                }
            }
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce.MediaExperience != null)
            {
                mce.MediaExperience.GoToFullScreen();
            }
        }
    }
}
