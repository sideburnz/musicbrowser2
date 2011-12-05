//using MusicBrowser.Entities;

//namespace MusicBrowser.Actions
//{
//    class ActionShowGroupTracksByGenre : baseActionCommand
//    {
//        private const string LABEL = "Tracks by Genre";
//        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

//        public ActionShowGroupTracksByGenre(Entity entity)
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//            Entity = entity;
//        }

//        public ActionShowGroupTracksByGenre()
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//        }

//        public override baseActionCommand NewInstance(Entity entity)
//        {
//            return new ActionShowGroupTracksByGenre(entity);
//        }

//        public override void DoAction(Entity entity)
//        {
//            Entity group = new Entity()
//            {
//                Kind = EntityKind.Group,
//                Title = "Tracks by Genre",
//                Label = "Tracks by Genre",
//                Path = "Tracks by Genre"
//            };
//            MusicBrowser.Application.GetReference().Navigate(group);
//        }
//    }
//}
