using System.Collections.Generic;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionDeleteCache : baseActionCommand
    {
        private const string LABEL = "Rebuild cache";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconDeleteCache";

        public ActionDeleteCache(baseEntity entity)
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

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionDeleteCache(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            bool confirmation;

            try
            {
                IList<DialogButtons> buttons = new List<DialogButtons>();
                buttons.Add(DialogButtons.Yes);
                buttons.Add(DialogButtons.No);

                DialogResult response =
                   Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                        ("This will delete the cache and close the application to force the cache to rebuild.",
                        "Rebuild Cache",
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
                Application.GetReference().Session().Close();
            }
        }
    }
}
