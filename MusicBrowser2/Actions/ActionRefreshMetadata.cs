using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using Microsoft.MediaCenter;

namespace MusicBrowser.Actions
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

            if (Util.Helper.InheritsFrom<Container>(entity))
            {
                try
                {
                    IList<DialogButtons> buttons = new List<DialogButtons>();
                    buttons.Add(DialogButtons.Yes);
                    buttons.Add(DialogButtons.No);

                    DialogResult response =
                       Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                            ("Fresh child metadata aswell",
                            "Refresh Metadata",
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
