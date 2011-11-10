﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    public class ActionPlayRandomPopularLastFM : baseActionCommand
    {
        private const string LABEL = "Play Random Last.fm";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconLastFM";

        public ActionPlayRandomPopularLastFM(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
            Entity = entity;
        }

        public ActionPlayRandomPopularLastFM()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders");
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlayRandomPopularLastFM(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", "random tracks with the high playcounts on Last.fm");
            CommonTaskQueue.Enqueue(new PlaylistProvider("cmdlastfmpopular", entity), true);   
        }
    }
}