﻿using MusicBrowser.Entities;
using System.IO;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MusicBrowser.Actions
{
    class ActionOpen : baseActionCommand
    {
        private const string LABEL = "Open";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconOpen";

        public ActionOpen(baseEntity entity)
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
                Available = !entity.InheritsFrom<View>() && InheritsFrom<Container>(entity);
            }
        }

        public ActionOpen()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionOpen(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            MusicBrowser.Application.GetReference().Navigate(entity);
        }

    }
}
