//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MusicBrowser.Engines.Transport;
//using MusicBrowser.Entities;
//using MusicBrowser.Models;

//namespace MusicBrowser.Actions
//{
//    public class ActionShowGroupBy : baseActionCommand
//    {
//        private const string LABEL = "Group By";
//        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconGroup";

//        public ActionShowGroupBy(baseEntity entity)
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//            Entity = entity;
//            KeepMenuShowingAfterExecution = true;
//        }

//        public ActionShowGroupBy()
//        {
//            Label = LABEL;
//            IconPath = ICON_PATH;
//            KeepMenuShowingAfterExecution = true;
//        }

//        public override baseActionCommand NewInstance(baseEntity entity)
//        {
//            return new ActionShowGroupBy(entity);
//        }

//        public override void DoAction(baseEntity entity)
//        {
//            baseEntity e = new Entity();
//            e.Kind = EntityKind.GroupBy;
//            e.Title = "Group By";

//            ActionShowActions action = new ActionShowActions(e);
//            action.Invoke();
//        }
//    }
//}
