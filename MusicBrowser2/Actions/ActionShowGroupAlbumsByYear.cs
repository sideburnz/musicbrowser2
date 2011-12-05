//using MusicBrowser.Entities;

//namespace MusicBrowser.Actions
//{
//    class ActionShowGroupAlbumsByYear : baseActionCommand
//    {
//        private const string LABEL = "Albums By Year";
//        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";

//        public ActionShowGroupAlbumsByYear(baseEntity entity)
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//            Entity = entity;
//        }

//        public ActionShowGroupAlbumsByYear()
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//        }

//        public override baseActionCommand NewInstance(baseEntity entity)
//        {
//            return new ActionShowGroupAlbumsByYear(entity);
//        }

//        public override void DoAction(baseEntity entity)
//        {
//            baseEntity group = new baseEntity()
//            {
//                Kind = EntityKind.Group,
//                Title = "Albums by Year",
//                Label = "Albums by Year",
//                Path = "Albums by Year"
//            };
//            MusicBrowser.Application.GetReference().Navigate(group);
//        }
//    }
//}
