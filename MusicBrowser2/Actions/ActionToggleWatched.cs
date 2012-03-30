using System;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    class ActionToggleWatched : baseActionCommand
    {
        private const string LABEL = "Toggle Viewed Status";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconNew";

        public ActionToggleWatched(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = entity.InheritsFrom<Video>();
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
            entity.TimesPlayed = 0;
            entity.UpdateCache();
        }

    }
}
