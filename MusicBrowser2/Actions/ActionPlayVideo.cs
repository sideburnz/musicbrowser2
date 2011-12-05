using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;

namespace MusicBrowser.Actions
{
    public class ActionPlayVideo : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayVideo(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayVideo()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayVideo(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (System.IO.Directory.Exists(entity.Path))
            {
                if (Util.Helper.IsDVD(entity.Path))
                {
                    mce.PlayMedia(MediaType.Dvd, entity.Path, false);
                }
                else
                {
                    // refer it on to a more specialist Play action
                    ActionPlayFolder a = new ActionPlayFolder(entity);
                    a.Invoke();
                    return;
                }
            }
            else
            {
                mce.PlayMedia(MediaType.Video, entity.Path, false);
            }
            mce.MediaExperience.GoToFullScreen();
            //TODO: this should detect when the video stops to "press" the back button
        }
    }
}
