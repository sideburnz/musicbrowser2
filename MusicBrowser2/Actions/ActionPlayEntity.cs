using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Transport;
using MusicBrowser.Entities;

namespace MusicBrowser.Actions
{
    class ActionPlayEntity : baseActionCommand
    {
        public ActionPlayEntity(Entity entity) : base(entity)
        {
            Label = "Play";
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";
        }

        public override void DoAction(Entity entity)
        {
            Transport.GetTransport().Play(false, entity.Path);
        }
    }
}
