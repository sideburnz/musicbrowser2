using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Actions
{
    class ActionRateLess : baseActionCommand
    {
        private const string LABEL = "Rate -1 Star";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconDown";

        public ActionRateLess(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionRateLess()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionRateLess(entity);
        }

        public override void DoAction(Entity entity)
        {
            ActionRate action = new ActionRate(entity);
            action.Rating = entity.Rating - 20;
            if (action.Rating <= 0) { action.Rating = 0; }
            action.Invoke();
        }
    }
}
