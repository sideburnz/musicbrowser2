using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Actions
{
    public class ActionDeleteCache : baseActionCommand
    {
        private const string LABEL = "Reset cache";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconDeleteCache";

        public ActionDeleteCache(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionDeleteCache()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionDeleteCache(entity);
        }

        public override void DoAction(Entity entity)
        {
            //TODO: prompt the user

            Models.UINotifier.GetInstance().Message = "resetting cache";
            CacheEngineFactory.GetEngine().Clear();
            InMemoryCache.GetInstance().Clear();
        }
    }
}
