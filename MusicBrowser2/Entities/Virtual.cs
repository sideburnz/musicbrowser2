using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.Text;
using MusicBrowser.Engines.Virtuals;
using MusicBrowser.Models;
using MusicBrowser.Util;
using Microsoft.MediaCenter;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Virtual : Container
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

            List<baseEntity> Tracks = new List<baseEntity>();
            List<baseEntity> Videos = new List<baseEntity>();

            foreach (baseEntity entity in entities)
            {
                if (entity.InheritsFrom<Track>()) { Tracks.Add(entity); }
                if (entity.InheritsFrom<Video>()) { Videos.Add(entity); }
            }

            if (Tracks.Count > 0 && Videos.Count == 0)
            {
                if (shuffle)
                {
                    Tracks = Tracks.Shuffle<baseEntity>().ToList();
                }
                Engines.Transport.TransportEngineFactory.GetEngine().Play(false, Tracks.Select(item => item.Path));

                return;
            }
            if (Tracks.Count == 0 && Videos.Count > 0)
            {
                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                MediaCollection collection = new MediaCollection();
                foreach (baseEntity e in Videos)
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
