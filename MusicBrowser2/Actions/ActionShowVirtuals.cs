using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
using MusicBrowser.Models;

namespace MusicBrowser.Actions
{
    public class ActionShowVirtuals : baseActionCommand
    {
        private const string LABEL = "Virtual Folders";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconOpen";

        public ActionShowVirtuals(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            KeepMenuShowingAfterExecution = true;
        }

        public ActionShowVirtuals()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            KeepMenuShowingAfterExecution = true;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionShowVirtuals(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            Models.VirtualsModel.GetInstance.Context = entity;
            Models.VirtualsModel.GetInstance.Visible = true;
        }
    }
}
