using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.Text;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;
using MusicBrowser.Providers;
using MusicBrowser.Util;
using System.IO;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Folder : Container
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
            Type thisType = this.GetType();

            if (thisType == typeof(Folder))
            {
                IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(Path);
                int music = 0;
                int video = 0;
                foreach (FileSystemItem item in items)
                {
                    Helper.knownType type = Helper.getKnownType(item);
                    if (type == Helper.knownType.Track) { music++; }
                    else if (type == Helper.knownType.Video) { video++; }
                }
                if (music >= video) { thisType = typeof(Album); }
                else { thisType = typeof(Season); }
            }

            if (thisType == typeof(Album) || thisType == typeof(Artist) || thisType == typeof(Genre))
            {
                MusicCollection m = new MusicCollection();
                m.Path = Path;
                m.Play(queue, shuffle);
            }

            if (thisType == typeof(Season) || thisType == typeof(Show))
            {
                VideoCollection v = new VideoCollection();
                v.Path = Path;
                v.Play(queue, shuffle);
            }

        }
    }
}
