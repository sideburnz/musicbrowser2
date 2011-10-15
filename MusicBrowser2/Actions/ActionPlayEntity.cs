using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Transport;

namespace MusicBrowser.Actions
{
    class ActionPlayEntity : baseActionCommand
    {
        public ActionPlayEntity()
        {
            Label = "Play";
            IconPath = "resx://MusicBrowser/MusicBrowser.Resources/IconPlay";
        }

        public void DoAction(Entities.Entity entity)
        {
            Transport.GetTransport().Play(false, entity.Path);
        }
    }
}
