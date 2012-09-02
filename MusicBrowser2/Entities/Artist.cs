using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MusicBrowser.Engines.ViewState;
using MusicBrowser.MediaCentre;
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

        public override IViewState ViewState
        {
            get
            {
                IViewState view = base.ViewState;
                view.DefaultSort = "[ReleaseDate:sort]";
                return view;
            }
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
            // track play progress for restart
            ProgressRecorder.Start();
        }

        public override List<string> SortFields
        {
            get
            {
                return new List<string>
                           {
                               "ReleaseDate",
                               "Title",
                               "Filename",
                               "Added",
                               "Duration",
                           };
            }
        }
    }
}
