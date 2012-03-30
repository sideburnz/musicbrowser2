using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.MediaCenter;
using MusicBrowser.Engines.Views;
using MusicBrowser.Models;
using MusicBrowser.Util;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Virtual : Container
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/MusicBrowser2"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override void Play(bool queue, bool shuffle)
        {
            IView view = Views.Fetch(Title);
            EntityCollection entities = view.Items;

            List<baseEntity> tracks = new List<baseEntity>();
            List<baseEntity> videos = new List<baseEntity>();

            foreach (baseEntity entity in entities)
            {
                if (entity.InheritsFrom<Track>()) { tracks.Add(entity); }
                if (entity.InheritsFrom<Video>()) { videos.Add(entity); }
            }

            if (tracks.Count > 0 && videos.Count == 0)
            {
                if (shuffle)
                {
                    tracks = tracks.Shuffle().ToList();
                }
                Engines.Transport.TransportEngineFactory.GetEngine().Play(false, tracks.Select(item => item.Path));

                return;
            }
            if (tracks.Count == 0 && videos.Count > 0)
            {
                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                MediaCollection collection = new MediaCollection();
                foreach (baseEntity e in videos)
                {
                    collection.AddItem(e.Path);
                    collection[collection.Count - 1].FriendlyData.Add("Title", System.IO.Path.GetFileNameWithoutExtension(e.Path));
                }
                mce.PlayMedia(MediaType.MediaCollection, collection, false);
                mce.MediaExperience.GoToFullScreen();
                return;
            }

            UINotifier.GetInstance().Message = "Cannot play lists of mixed types";

        }
    }
}
