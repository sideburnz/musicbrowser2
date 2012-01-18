using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using System.IO;

namespace MusicBrowser.Actions
{
    class ActionShuffle : baseActionCommand
    {
        private const string LABEL = "Shuffle";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconShuffle";

        public ActionShuffle(baseEntity entity)
        {
            Label = LABEL + " " + entity.Kind;
            IconPath = ICON_PATH;
            Entity = entity;

            if (InheritsFrom<Video>(entity) && Directory.Exists(entity.Path))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetFolderContents(entity.Path);
                int hits = 0;
                foreach (FileSystemItem item in items)
                {
                    if (Util.Helper.getKnownType(item) == Util.Helper.knownType.Video)
                    {
                        hits++;
                        if (hits > 1)
                        {
                            Available = true;
                            return;
                        }
                    }
                }
                Available = false;
            }
            else
            {
                Available = InheritsFrom<Container>(entity);
            }

        }

        public ActionShuffle()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShuffle(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            entity.Play(false, true);
        }
    }
}
