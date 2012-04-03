using System;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.MediaCentre;

namespace MusicBrowser.Actions
{
    public class ActionResume : baseActionCommand
    {
        private const string LABEL = "Resume";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconResume";

        public ActionResume(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = (InheritsFrom<Video>(entity)) && ((Video)entity).Progress > 0; 
        }

        public ActionResume()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionResume(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Video v = (Video) entity;

            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (System.IO.Directory.Exists(entity.Path))
            {
                if (Util.Helper.IsDVD(entity.Path))
                {
                    entity.MarkPlayed();
                    mce.PlayMedia(MediaType.Dvd, entity.Path, false);
                    mce.MediaExperience.Transport.Position = new TimeSpan(0, 0, v.Progress);
                }
            }
            else
            {
                entity.MarkPlayed();
                mce.PlayMedia(MediaType.Video, entity.Path, false);
                mce.MediaExperience.Transport.Position = new TimeSpan(0, 0, v.Progress);
            }

            mce.MediaExperience.GoToFullScreen();
            ProgressRecorder.Register(v);
        }
    }
}
