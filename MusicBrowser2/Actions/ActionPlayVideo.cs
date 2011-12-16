﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using MusicBrowser.MediaCentre;

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
            Available = (InheritsFrom<Video>(entity)); 
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
                    entity.MarkPlayed();
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
                entity.MarkPlayed();
                mce.PlayMedia(MediaType.Video, entity.Path, false);
            }
            mce.MediaExperience.GoToFullScreen();
            ProgressRecorder.Register(entity);
        }
    }
}
