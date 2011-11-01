using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Actions
{
    public class ActionRefreshMetadata : baseActionCommand
    {
        private const string LABEL = "Refresh Metadata";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconRefresh";

        public ActionRefreshMetadata(Entity entity)
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

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionRefreshMetadata(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("refreshing metadata for {0}", entity.Title);
            CommonTaskQueue.Enqueue(new ForceMetadataRefreshProvider(entity), true);
        }
    }
}
