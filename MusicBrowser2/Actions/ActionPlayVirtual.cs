//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MusicBrowser.Providers;
//using MusicBrowser.Entities;
//using MusicBrowser.Providers.Background;
//using MusicBrowser.Engines.Transport;
//using MusicBrowser.Engines.Cache;
//using Microsoft.MediaCenter;

//namespace MusicBrowser.Actions
//{
//    public class ActionPlayVirtual : baseActionCommand
//    {
//        private const string LABEL = "Play";
//        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

//        public ActionPlayVirtual(baseEntity entity)
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//            Entity = entity;
//        }

//        public ActionPlayVirtual()
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//        }

//        public override baseActionCommand NewInstance(baseEntity entity)
//        {
//            return new ActionPlayVirtual(entity);
//        }

//        public override void DoAction(baseEntity entity)
//        {
//            Models.UINotifier.GetInstance().Message = String.Format("playing 'virtual items' is currently not supported");

//            return;


//            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", entity.Title);
//            if (entity.Kind == EntityKind.Virtual)
//            {
//                EntityCollection entities = null;
//                switch (entity.Path.ToLower())
//                {
//                    //TODO: redo this group by logic 
//                    case "tracks by genre":
//                        {
//                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track, "Genre", entity.Title);
//                            break;
//                        }
//                    case "albums by year":
//                        {
//                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "Year", entity.Title);
//                            break;
//                        }
//                    case "albums":
//                        {
//                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "", entity.Title);
//                            break;
//                        }
//                }
//                if (entities.Count > 0)
//                {
//                    TransportEngineFactory.GetEngine().Play(false, entities.FirstOrDefault<Entity>().Path);
//                    entities.RemoveAt(0);
//                }
//                foreach (Entity e in entities)
//                {
//                    TransportEngineFactory.GetEngine().Play(true, e.Path);
//                }
//            }
//            else if (entity.Kind == EntityKind.Video || entity.Kind == EntityKind.Movie || entity.Kind == EntityKind.Episode)
//            {
//                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
//                if (Util.Helper.IsDVD(entity.Path))
//                {
//                    mce.PlayMedia(MediaType.Dvd, entity.Path, false);
//                }
//                mce.PlayMedia(MediaType.Video, entity.Path, false);
//            }
//            else if (entity.Kind == EntityKind.Photo)
//            {
//                MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
//                mce.PlayMedia(MediaType.Unknown, entity.Path, false);
//            }
//            else
//            {
//                TransportEngineFactory.GetEngine().Play(false, entity.Path);
//            }

//            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
//        }
//    }
//}
