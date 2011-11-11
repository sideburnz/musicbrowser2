using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.Actions
{
    public class ActionPlay : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlay(Entity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionPlay()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(Entity entity)
        {
            return new ActionPlay(entity);
        }

        public override void DoAction(Entity entity)
        {
            Models.UINotifier.GetInstance().Message = String.Format("playing {0}", entity.Title);
            if (entity.Kind == EntityKind.Virtual)
            {
                EntityCollection entities = null;
                switch (entity.Path.ToLower())
                {
                    //TODO: redo this group by logic 
                    case "tracks by genre":
                        {
                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Track, "Genre", entity.Title);
                            break;
                        }
                    case "albums by year":
                        {
                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "Year", entity.Title);
                            break;
                        }
                    case "albums":
                        {
                            entities = InMemoryCache.GetInstance().DataSet.Filter(EntityKind.Album, "", entity.Title);
                            break;
                        }
                }
                if (entities.Count > 0)
                {
                    TransportEngineFactory.GetEngine().Play(false, entities.FirstOrDefault<Entity>().Path);
                    entities.RemoveAt(0);
                }
                foreach (Entity e in entities)
                {
                    TransportEngineFactory.GetEngine().Play(true, e.Path);
                }
            }
            else
            {
                TransportEngineFactory.GetEngine().Play(false, entity.Path);
            }

            MusicBrowser.MediaCentre.Playlist.AutoShowNowPlaying();
        }
    }
}
