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
    class ActionRateMore : baseActionCommand
    {
        private const string LABEL = "Rate +1 Star";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconUp";

        public ActionRateMore(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionRateMore()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionRateMore(entity);
        }

        public override void DoAction(Entity entity)
        {
            ActionRate action = new ActionRate(entity);
            action.Rating = entity.Rating + 20;
            if (action.Rating > 100) { action.Rating = 100; }
            action.Invoke();
        }
    }
}
