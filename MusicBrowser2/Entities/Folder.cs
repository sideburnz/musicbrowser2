using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Folder : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageFolder"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override void Play(bool queue, bool shuffle)
        {
            Type thisType = GetType();

            if (thisType == typeof(Folder))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(Path).FilterInternalFiles();
                int music = 0;
                int video = 0;
                foreach (FileSystemItem item in items)
                {
                    Helper.KnownType type = Helper.GetKnownType(item);
                    if (type == Helper.KnownType.Track) { music++; }
                    else if (type == Helper.KnownType.Video) { video++; }
                }
                thisType = music >= video ? typeof(Album) : typeof(Season);
            }

            if (thisType == typeof(Album) || thisType == typeof(Artist) || thisType == typeof(Genre))
            {
                MusicCollection m = new MusicCollection {Path = Path};
                m.Play(queue, shuffle);
            }

            if (thisType == typeof(Season) || thisType == typeof(Show))
            {
                VideoCollection v = new VideoCollection {Path = Path};
                v.Play(queue, shuffle);
            }

        }
    }
}
