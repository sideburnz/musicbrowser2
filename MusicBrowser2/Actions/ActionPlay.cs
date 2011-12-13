using MusicBrowser.Entities;

// this is a wrapper action around the specific Play actions, this allows config to just say Play
// and for the code for the play actions to be simple

namespace MusicBrowser.Actions
{
    public class ActionPlay : baseActionCommand
    {
        private const string LABEL = "Play";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

        public ActionPlay(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = InheritsFrom<Item>(entity);
        }

        public ActionPlay()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlay(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            baseActionCommand action = new ActionNoOperation();

            if (InheritsFrom<Collection>(entity))
            {
                action = new ActionPlayFolder(entity);
            }
            if (InheritsFrom<Music>(entity))
            {
                action = new ActionPlayMusic(entity);
            }
            if (InheritsFrom<Video>(entity))
            {
                action = new ActionPlayVideo(entity);
            }
            if (InheritsFrom<Photo>(entity))
            {
                action = new ActionPlayImage(entity);
            }

            action.Invoke();
        }
    }
}
