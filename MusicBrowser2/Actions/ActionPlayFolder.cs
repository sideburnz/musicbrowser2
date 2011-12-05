using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;
using Microsoft.MediaCenter;

namespace MusicBrowser.Actions
{
    public class ActionPlayFolder : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlayFolder(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlayFolder()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayFolder(entity);
        }

        public override void DoAction(baseEntity entity)
        {

            // music folders 
            //switch (entity.Kind)
            //{
            //    case EntityKind.Album:
            //    case EntityKind.Artist:
            //    case EntityKind.Genre:
            //        TransportEngineFactory.GetEngine().Play(false, entity.Path);
            //        return;
            //    case EntityKind.PhotoAlbum:
            //        Models.UINotifier.GetInstance().Message = String.Format("playing image folders doesn't work");
            //        return;
            //}

            Models.UINotifier.GetInstance().Message = String.Format("playing videos folders doesn't always work");

            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            MediaCollection collection = new MediaCollection();
            string lastitem = string.Empty;

            // it's a playlist of files so
            // get them, weed out the non-video, sort them and play them
            List<FileSystemItem> candidateitems = FileSystemProvider.GetAllSubPaths(entity.Path).
                OrderBy(item => item.Name).
                ToList();

            foreach (FileSystemItem item in candidateitems)
            {
                var t = Util.Helper.getKnownType(item);
                if (t == Util.Helper.knownType.Video) // || t == Util.Helper.knownType.Track || t == Util.Helper.knownType.Image)
                {
                    collection.AddItem(item.FullPath);
                    collection[collection.Count - 1].FriendlyData.Add("Title", entity.Title + " (" + Path.GetFileNameWithoutExtension(item.Name) + ")");
                    lastitem = item.FullPath;
                }
            }

            // if there's only 1 item, just play it (this may go wrong if it's not a video)
            if (collection.Count == 1)
            {
                mce.PlayMedia(MediaType.Video, lastitem, false);
                mce.MediaExperience.GoToFullScreen();
                return;
            }

            Models.UINotifier.GetInstance().Message = String.Format("playing {0} folder items", collection.Count());
            mce.PlayMedia(MediaType.MediaCollection, collection, false);
//            mce.MediaExperience.GoToFullScreen();
        }
    }
}
