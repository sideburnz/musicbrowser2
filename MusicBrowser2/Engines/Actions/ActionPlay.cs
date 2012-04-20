using System;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.MediaCentre;

// this is a wrapper action around the specific Play actions, this allows config to just say Play
// and for the code for the play actions to be simple

namespace MusicBrowser.Engines.Actions
{
    public class ActionPlay : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlay(baseEntity entity)
        {
            Label = LABEL + " " + entity.Kind;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlay()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlay(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            // play the item
            entity.Play(false, false);

            // if we have a progress indicator, use it to "resume" play
            if (entity.PlayState.Progress > 0)
            {
                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                if (mce != null)
                {
                    mce.MediaExperience.Transport.Position = new TimeSpan(0, 0, (int)entity.PlayState.Progress);
                }
            }

            // register for progress recording
            ProgressRecorder.Register(entity);

            // update the play count and make sure the cache is updated.
            entity.PlayState.Play();
            entity.UpdateCache();
        }
    }
}
