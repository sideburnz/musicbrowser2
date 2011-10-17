using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Actions;
using MusicBrowser.Entities;

namespace MusicBrowser.Models
{
    public class ActionsModel : BaseModel
    {
        #region singleton
        static ActionsModel _instance;
        
        public static ActionsModel GetInstance
        {
            get 
            {
            if (_instance != null) return _instance;
            _instance = new ActionsModel();
            return _instance;
            }
        }
        #endregion

        public bool _visible = false;

        public bool Visible
        {
            get 
            { 
                return _visible; 
            }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    FirePropertyChanged("Visible");
                }
            }
        }

        //TODO: move the logic for determining which acitions apply to which entities to this
        //TODO: workout how to remove [Open] if the entity is already open
        //public baseActionCommand GetDefaultAction(EntityKind kind)


        //TODO : somehow execute a default action [usually open or play]
        //TODO : make this configurable
        public static VirtualList GetActionList(Entity entity)
        {
            VirtualList actions = new VirtualList();

            switch (entity.Kind)
            {
                case EntityKind.Track:
                    actions.Add(new ActionPlayEntity(entity));
                    actions.Add(new ActionOpenEntity(entity));
                    actions.Add(new ActionRefreshMetadata(entity));
                    actions.Add(new ActionCloseMenu(null));
                    break;
                case EntityKind.Genre:
                    actions.Add(new ActionPlayEntity(entity));
                    actions.Add(new ActionOpenEntity(entity));
                    actions.Add(new ActionRefreshMetadata(entity));
                    actions.Add(new ActionCloseMenu(null));
                    break;
                case EntityKind.Album:
                    actions.Add(new ActionPlayEntity(entity));
                    actions.Add(new ActionOpenEntity(entity));
                    actions.Add(new ActionRefreshMetadata(entity));
                    actions.Add(new ActionCloseMenu(null));
                    break;
                case EntityKind.Artist:
                    actions.Add(new ActionOpenEntity(entity));
                    actions.Add(new ActionPlayEntity(entity));
                    actions.Add(new ActionRefreshMetadata(entity));
                    actions.Add(new ActionCloseMenu(null));
                    break;
                case EntityKind.Home:
                    actions.Add(new ActionPlayFavourites(entity));
                    actions.Add(new ActionCloseMenu(null));
                    break;
            }

            return actions;
        }

        public static baseActionCommand GetDefaultAction(Entity entity)
        {
            return (baseActionCommand)GetActionList(entity)[0];
        }

    }

}
