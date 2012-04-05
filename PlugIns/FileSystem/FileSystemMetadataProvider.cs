using System.Collections.Generic;
using System.IO;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Metadata
{
//    // populates children and all descendants

    public class FileSystemMetadataProvider : baseMetadataProvider
    {
        public FileSystemMetadataProvider()
        {
            Name = "FileSystemMetaDataProvider";
            MinDaysBetweenHits = 1;
            MaxDaysBetweenHits = 10;
            RefreshPercentage = 25;
        }

        public override bool CompatibleWith(baseEntity type)
        {
            return type.InheritsFrom<Folder>();
        }
        
        public override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Directory.Exists(dto.Path)) { return false; }
            return true;
        }

        public override ProviderOutcome DoWork(baseEntity dto)
        {
            int duration = 0;
            Dictionary<string, int> children = new Dictionary<string, int>();

            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(dto.Path).FilterInternalFiles();
            foreach (FileSystemItem item in items)
            {
                baseEntity e = EntityFactory.GetItem(item);
                if (e == null) { continue; }

                if (e.InheritsFrom<Video>() || e.InheritsFrom<Music>())
                {
                    duration += e.Duration;
                }

                if (children.ContainsKey(e.Kind))
                {
                    children[e.Kind]++;
                }
                else
                {
                    children.Add(e.Kind, 1);
                }
            }

            dto.Duration = duration;
            ((Container)dto).Children = children;

            return ProviderOutcome.Success;
        }
    }
}
