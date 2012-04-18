using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MusicBrowser.Providers;
using MusicBrowser.Util;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Album : Folder
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageAlbum"; }
        }

        protected override string DefaultSort
        {
            get { return "[Track#:sort]"; }
        }

        protected override string DefaultFormat
        {
            get
            {
                string title = Util.Config.GetInstance().GetStringSetting("Entity.Album.Format");
                if (string.IsNullOrEmpty(title))
                {
                    return "([ReleaseYear]) [Title]";
                }
                return base.DefaultFormat;
            }
        }

        public override string Information
        {
            get
            {
                return CalculateInformation("", "Track");
            }
        }

        [DataMember]
        public string MusicBrainzID { get; set; }
        [DataMember]
        public int Listeners { get; set; }
        [DataMember]
        public int LastFMPlayCount { get; set; }
        [DataMember]
        public string AlbumArtist { get; set; }

        public override void Play(bool queue, bool shuffle)
        {
            // get a list of all of the tracks in contect
            IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(Path)
                .FilterInternalFiles()
                .Where(item => Helper.GetKnownType(item) == Helper.KnownType.Track);

            // convert them to entities so they can be sorted
            EntityCollection entityCollection = new EntityCollection();
            entityCollection.AddRange(items);

            // shuffle or sort
            if (shuffle)
            {
                entityCollection.Shuffle();
            }
            else
            {
                entityCollection.Sort(SortField);
            }

            // play
            Engines.Transport.TransportEngineFactory.GetEngine().Play(queue, entityCollection.Select(item => item.Path));
        }
    }
}
