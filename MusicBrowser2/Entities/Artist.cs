using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MusicBrowser.Providers;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Artist : Folder
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageArtist"; }
        }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Album", "Track");
            }
        }

        protected override string DefaultSort
        {
            get { return "[ReleaseDate:sort]"; }
        }

        [DataMember]
        public string MusicBrainzID { get; set; }
        [DataMember]
        public int Listeners { get; set; }
        [DataMember]
        public int LastFMPlayCount { get; set; }

        public override void Play(bool queue, bool shuffle)
        {
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(Path);
            List<string> playlist = (from item in items where Util.Helper.GetKnownType(item) == Util.Helper.KnownType.Track select item.FullPath).ToList();

            if (shuffle)
            {
                Util.Helper.Shuffle(playlist);
            }

            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, playlist);
        }
    }
}
