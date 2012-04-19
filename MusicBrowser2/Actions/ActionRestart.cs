using System;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.MediaCentre;

namespace MusicBrowser.Actions
{
    public class ActionRestart : baseActionCommand
    {
        private const string LABEL = "Restart";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRestart";

        public ActionRestart(baseEntity entity)
        {
            Label = LABEL + " " + entity.Kind;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = entity.InheritsFrom<Video>() && entity.PlayState.Progress > 0;
        }

        public ActionRestart()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionRestart(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            // we're restarting, make sure any existing progress indicator is removed
            entity.SetProgress(0);

            // play the item
            entity.Play(false, false);

            // make sure it plays from the beginning
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce != null)
            {
                mce.MediaExperience.Transport.Position = new TimeSpan(0, 0, 0);
            }

            // register for progress recording
            ProgressRecorder.Register(entity);

            // update the play count and make sure the cache is updated.
            entity.PlayState.Play();
            entity.UpdateCache();
        }
    }
}
