using System.Collections.Generic;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Engines.Actions
{
    public class ActionRefreshMetadata : baseActionCommand
    {
        private const string LABEL = "Refresh Metadata";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRefresh";

        public ActionRefreshMetadata(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = !entity.InheritsFrom<View>();
        }

        public ActionRefreshMetadata()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionRefreshMetadata(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            bool confirmation = false;

            if (entity.InheritsFrom<Container>())
            {
                try
                {
                    IList<DialogButtons> buttons = new List<DialogButtons>();
                    buttons.Add(DialogButtons.Yes);
                    buttons.Add(DialogButtons.No);

                    DialogResult response =
                       Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                            ("Refresh child data too?",
                            "Refresh Data",
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
            }

            CommonTaskQueue.Enqueue(new ForceMetadataRefreshProvider(entity, confirmation), true);
        }
    }
}
