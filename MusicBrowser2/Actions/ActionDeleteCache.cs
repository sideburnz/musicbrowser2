using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;

namespace MusicBrowser.Actions
{
    public class ActionDeleteCache : baseActionCommand
    {
        private const string LABEL = "Rebuild cache";
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
            bool confirmation = false;

            try
            {
                IList<DialogButtons> buttons = new List<DialogButtons>();
                buttons.Add(DialogButtons.Yes);
                buttons.Add(DialogButtons.No);

                DialogResult response =
                   Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                        ("Are you sure you want to rebuild the cache?",
                        "Confirm",
                        buttons,
                        30,
                        true,
                        "");

                confirmation = (response == DialogResult.Yes);
            }
            catch
            {
                confirmation = true;
            }

            if (confirmation)
            {
                Models.UINotifier.GetInstance().Message = "removing cached data";
                CacheEngineFactory.GetEngine().Clear();
                InMemoryCache.GetInstance().Clear();
            }
        }
    }
}
