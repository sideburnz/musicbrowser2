using MusicBrowser.Entities;

// this is a wrapper action around the specific Play actions, this allows config to just say Play
// and for the code for the play actions to be simple

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
            baseActionCommand action;

            switch (entity.Kind)
            {
                case EntityKind.Album:
                case EntityKind.Artist:
                case EntityKind.Folder:
                case EntityKind.Genre:
                case EntityKind.PhotoAlbum:
                case EntityKind.Series:
                case EntityKind.Show:
                    action = new ActionPlayFolder(entity);
                    break;
                case EntityKind.Episode:
                case EntityKind.Movie:
                    action = new ActionPlayVideo(entity);
                    break;
                case EntityKind.Photo:
                    action = new ActionPlayImage(entity);
                    break;
                case EntityKind.Playlist:
                case EntityKind.Track:
                    action = new ActionPlayMusic(entity);
                    break;
                case EntityKind.Group:
                case EntityKind.GroupBy:
                case EntityKind.Virtual:
                    action = new ActionPlayVirtual(entity);
                    break;
                default:
                    action = new ActionNoOperation();
                    break;
            }

            action.Invoke();
        }
    }
}
