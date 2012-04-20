using System.Collections.Generic;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionCleanCache : baseActionCommand
    {
        private const string LABEL = "Clean cache";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconDeleteCache";

        public ActionCleanCache(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCleanCache()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionCleanCache(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            bool confirmation = false;

            try
            {
                IList<DialogButtons> buttons = new List<DialogButtons>();
                buttons.Add(DialogButtons.Yes);
                buttons.Add(DialogButtons.No);

                DialogResult response =
                   Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                        ("This may take some time and the application will not be usable whilst this is running.",
                        "Clean the cache?",
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
                Models.UINotifier.GetInstance().Message = "validating items in the cache";
                CacheEngineFactory.GetEngine().Scavenge();
                Models.UINotifier.GetInstance().Message = "compressing cache";
                CacheEngineFactory.GetEngine().Compress();
            }
        }
    }
}
