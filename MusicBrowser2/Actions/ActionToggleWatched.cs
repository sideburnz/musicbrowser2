using MusicBrowser.Entities;
using System.IO;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System;
using System.Collections;
using System.Collections.Generic;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Util;

namespace MusicBrowser.Actions
{
    class ActionToggleWatched : baseActionCommand
    {
        private const string LABEL = "Toggle Viewed Status";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconNew";

        public ActionToggleWatched(baseEntity entity)
        {
            if (entity.InheritsFrom<Music>())
            {
                Label = "Toggle Listened Status";
            }
            else
            {
                Label = LABEL;
            }
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionToggleWatched()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionToggleWatched(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            if (entity.LastPlayed > DateTime.Parse("1000-01-01"))
            {
                entity.LastPlayed = DateTime.Parse("1000-01-01").AddDays(-1.00);
            }
            else
            {
                entity.LastPlayed = DateTime.Parse("2000-01-01");
            }
            entity.UpdateCache();
        }

    }
}
